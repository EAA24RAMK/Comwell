using Core.Models;
using MongoDB.Driver;
using ServerAPI.Repositories;

namespace ServerAPI;

public class StudentPlanRepository : IStudentPlanRepository
{
    private readonly IMongoCollection<StudentPlan> _studentPlan;

    public StudentPlanRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _studentPlan = db.GetCollection<StudentPlan>("studentplan");
    }
    
    // Finder højeste ID og plusser med en, og giver den ID til ny plan
    // Opretter og indsætter ny plan i database
    public async Task CreateStudentPlanAsync(StudentPlan createPlan)
    {
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
    
}

    