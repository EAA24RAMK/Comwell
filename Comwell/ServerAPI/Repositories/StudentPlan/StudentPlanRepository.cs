using Core.Models;
using MongoDB.Driver;
using ServerAPI.Repositories;

namespace ServerAPI;

public class StudentPlanRepository : IStudentPlanRepository
{
    private readonly IMongoCollection<StudentPlan> _studentPlan;
    private readonly ITemplateRepository _templateRepo;

    public StudentPlanRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _studentPlan = db.GetCollection<StudentPlan>("studentplan");
        
        _templateRepo = new TemplateRepository(config);
    }
    
    // Finder højeste ID og plusser med en, og giver den ID til ny plan
    // Opretter og indsætter ny plan i database
    public async Task CreateStudentPlanAsync(StudentPlan createPlan)
    {
        // Hent template fra database
        var template = await _templateRepo.GetTemplateByIdAsync(createPlan.TemplateId);
        if (template == null)
            throw new Exception("Template not found");

        // Kopiér Goals fra template
        createPlan.Goals = template.Goals;

        // Sæt slutdato baseret på template-title
        var title = template.Title.ToLower();
        if (title.Contains("1. praktikperiode"))
            createPlan.PeriodEnd = createPlan.PeriodStart.AddDays(52 * 7);
        else if (title.Contains("2. praktikperiode"))
            createPlan.PeriodEnd = createPlan.PeriodStart.AddDays(43 * 7);
        else if (title.Contains("3. praktikperiode"))
            createPlan.PeriodEnd = createPlan.PeriodStart.AddDays(43 * 7);
        else if (title.Contains("afslutning"))
            createPlan.PeriodEnd = createPlan.PeriodStart.AddDays(4 * 7);
        else
            createPlan.PeriodEnd = createPlan.PeriodStart;

        // Beregn deadlines
        foreach (var goal in createPlan.Goals)
        {
            goal.Deadline = CalculateDeadline(createPlan.PeriodStart, goal.Title.ToLower(), title);
        }

        // Sæt ID
        int maxId = 0;
        var allPlans = await _studentPlan.Find(_ => true).ToListAsync();
        if (allPlans.Any())
        {
            maxId = allPlans.Max(t => t.Id);
        }
        createPlan.Id = maxId + 1;

        await _studentPlan.InsertOneAsync(createPlan);
    }

    
    // Henter alle elevplaner og returnerer dem i en liste
    public async Task<List<StudentPlan>> GetAllPlansAsync()
    {
        return await _studentPlan.Find(_ => true).ToListAsync();
    }

    // Henter en elevs planer ud fra Student ID returner i en liste
    public async Task<List<StudentPlan>> GetPlansByStudentAsync(int studentId)
    {
        return await _studentPlan.Find(p => p.StudentId == studentId).ToListAsync();
    }

    // Henter alle elevplaner fra et hotel, så køkkenchefen kan se sine elevers planer
    // Til at elever kun kan se deres egen plan
    public async Task<List<StudentPlan>> GetPlansByHotelAsync(string hotel)
    {
        var client = new MongoClient(_studentPlan.Database.Client.Settings);
        var userCollection = client
            .GetDatabase(_studentPlan.Database.DatabaseNamespace.DatabaseName)
            .GetCollection<User>("user");
        
        var students = await userCollection
            .Find(u => u.Hotel == hotel && u.Role == "Elev")
            .Project(u => u.Id)
            .ToListAsync();
        
        return await _studentPlan.Find(p => students.Contains(p.StudentId)).ToListAsync();
    }
    
    public async Task<StudentPlan?> GetPlanByIdAsync(int id)
    {
        return await _studentPlan.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task UpdateStudentPlanAsync(StudentPlan updatedPlan)
    {
        await _studentPlan.ReplaceOneAsync(p => p.Id == updatedPlan.Id, updatedPlan);
    }
    
    private DateTime CalculateDeadline(DateTime start, string goalTitle, string planTitle)
    {
        var praktik1Start = start;
        var praktik2Start = praktik1Start.AddDays(52 * 7);
        var praktik3Start = praktik2Start.AddDays(43 * 7);
        var afslutningStart = praktik3Start.AddDays(43 * 7);

        if (planTitle.Contains("1. praktik"))
        {
            if (goalTitle.Contains("inden første dag")) return praktik1Start.AddDays(-1);
            if (goalTitle.Contains("velkommen")) return praktik1Start.AddDays(7);
            if (goalTitle.Contains("information")) return praktik1Start.AddDays(14);
            if (goalTitle.Contains("sikkerhed")) return praktik1Start.AddMonths(1);
            if (goalTitle.Contains("samtaler")) return praktik1Start.AddDays(42);
            if (goalTitle.Contains("kurser")) return praktik1Start.AddMonths(2);
            if (goalTitle.Contains("faglige mål")) return praktik1Start.AddMonths(3);
            if (goalTitle.Contains("madspild")) return praktik1Start.AddMonths(3);
        }

        if (planTitle.Contains("2. praktik"))
        {
            if (goalTitle.Contains("evaluering")) return praktik2Start.AddDays(-1);
            if (goalTitle.Contains("interne") || goalTitle.Contains("faglige mål")) return praktik2Start.AddDays(43 * 7);
        }

        if (planTitle.Contains("3. praktik"))
        {
            if (goalTitle.Contains("evaluering")) return praktik3Start.AddDays(-1);
            if (goalTitle.Contains("interne") || goalTitle.Contains("faglige mål")) return praktik3Start.AddDays(43 * 7);
            if (goalTitle.Contains("klar-parat")) return afslutningStart.AddDays(-14);
        }

        if (planTitle.Contains("afslutning"))
        {
            if (goalTitle.Contains("evaluering") || goalTitle.Contains("interne"))
                return afslutningStart.AddDays(28);
        }

        return start;
    }
    
    // sletter en hel studentplan
    public async Task<bool> DeleteStudentPlanAsync(int id)
    {
        var result = await _studentPlan.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }

}

    