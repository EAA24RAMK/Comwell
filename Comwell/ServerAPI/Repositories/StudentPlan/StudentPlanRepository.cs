using Core.Models;
using MongoDB.Driver;
using ServerAPI.Repositories;

namespace ServerAPI;

/* Repository overblik
- Ansvarlig for alt, der har med elevplaner (StudentPlan) at gøre.
- Kommunikerer med MongoDB for at oprette, hente, opdatere og slette elevplaner.
- Bruger TemplateRepository og NotificationRepository til at hente templates og håndtere notifikationer. */
public class StudentPlanRepository : IStudentPlanRepository
{
    private readonly IMongoCollection<StudentPlan> _studentPlan; // Instansvariabel for StudentPlan-collection i MongoDB
    private readonly ITemplateRepository _templateRepo; // Instansvariabel til at hente templates
    private readonly INotificationRepository _notificationRepo; // Instansvariabel til at håndtere notifikationer
    private readonly IUserRepository _userRepo; // Instansvariabel til at hente brugere
    
    // Konstruktøren sørger for at etablere forbindelsen til MongoDB.
    // connection string og databasenavn læses fra appsettings.json.
    // Opretter adgang til både templates og notifikationer, så vi kan hente skabeloner og styre notifikationer.
    public StudentPlanRepository(IConfiguration config, ITemplateRepository templateRepo, INotificationRepository notificationRepo, IUserRepository userRepo)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _studentPlan = db.GetCollection<StudentPlan>("studentplan");
        
        _templateRepo = templateRepo;
        _notificationRepo = notificationRepo;
        _userRepo = userRepo;;
    }
    
    // Parametre: createPlan – den plan der skal oprettes.
    // Formål: Opretter en ny elevplan i databazsen. Sætter ID, deadlines, mål og skoleperioder baseret på skabelonen.
    public async Task CreateStudentPlanAsync(StudentPlan createPlan)
    {
        // Henter den tilknyttede template fra database
        var template = await _templateRepo.GetTemplateByIdAsync(createPlan.TemplateId);
        if (template == null)
            throw new Exception("Template not found");

        // Kopier målene fra template over i planen
        createPlan.Goals = template.Goals;

        // Sætter slutdato baseret på hvilken template det er
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

        // Beregner deadlines for hvert mål ved at kalde CalculateDeadline()
        foreach (var goal in createPlan.Goals)
        {
            goal.Deadline = CalculateDeadline(createPlan.PeriodStart, goal.Title.ToLower(), title);
        }

        // Finder højeste ID og plusser med en, og giver den ID til den nye plan
        int maxId = 0;
        var allPlans = await _studentPlan.Find(_ => true).ToListAsync();
        if (allPlans.Any())
        {
            maxId = allPlans.Max(t => t.Id);
        }
        createPlan.Id = maxId + 1;
        
        createPlan.SchoolPeriods = new List<SchoolPeriod>(); // Opretter tom liste for skoleperioder
        
        // Henter alle skoleperioder fra alle elevplaner
        int maxSchoolId = 0;
        var allSchoolPeriods = allPlans 
            .Where(p => p.SchoolPeriods != null) //Filtrerer planer der har skoleperioder
            .SelectMany(p => p.SchoolPeriods!) // Samler alle skoleperioder i en liste
            .ToList();

        // Hvis der findes skoleperioder i databasen, finder højeste ID 
        if (allSchoolPeriods.Any())
        {
            maxSchoolId = allSchoolPeriods.Max(sp => sp.Id);
        }

        // Tilføjer skoleperiode baseret på praktikperioden og giver dem unikt Id
        if (title.Contains("1. praktikperiode"))
        {
            createPlan.SchoolPeriods.Add(new SchoolPeriod
            {
                Id = maxSchoolId + 1,
                Title = "1. skoleperiode (10 uger)",
                TemplateId = createPlan.TemplateId,
                DurationWeeks = 10
            });
        }
        else if (title.Contains("2. praktikperiode"))
        {
            createPlan.SchoolPeriods.Add(new SchoolPeriod
            {
                Id = maxSchoolId + 1,
                Title = "2. skoleperiode (10 uger)",
                TemplateId = createPlan.TemplateId,
                DurationWeeks = 10
            });
        }
        else if (title.Contains("3. praktikperiode"))
        {
            createPlan.SchoolPeriods.Add(new SchoolPeriod
            {
                Id = maxSchoolId + 1,
                Title = "3. skoleperiode (7 uger)",
                TemplateId = createPlan.TemplateId,
                DurationWeeks = 7
            });
        }
        else if (title.Contains("afslutning"))
            createPlan.SchoolPeriods.Add(new SchoolPeriod
            {
                Id = maxSchoolId + 1,
                Title = "Fagprøve",
                TemplateId = null,
                DurationWeeks = 2
            });

        await _studentPlan.InsertOneAsync(createPlan);
    }
    
    // Returnerer: En liste med alle elevplaner i databasen.
    // Formål: Henter alle elevplaner, bruges i Elevplaner-siden.
    public async Task<List<StudentPlan>> GetAllPlansAsync()
    {
        return await _studentPlan.Find(_ => true).ToListAsync();
    }

    // Returnerer: En liste med alle planer for en specifik elev.
    // Parametre: studentId – ID på eleven.
    // Formål: Henter alle planer for en bestemt elev, bruges i Elevplan-siden og dropdown på elevplaner-siden..
    public async Task<List<StudentPlan>> GetPlansByStudentAsync(int studentId)
    {
        return await _studentPlan.Find(p => p.StudentId == studentId).ToListAsync();
    }
    
    // Returnerer: En liste af elevplaner tilknyttet et bestemt hotel.
    // Parametre: hotel – Navn på hotellet.
    // Formål: Bruges fx af HR til at få overblik over alle elever på et specifikt hotel.
    public async Task<List<StudentPlan>> GetPlansByHotelAsync(string hotel)
    {
        var allUsers = await _userRepo.GetAllAsync();
        var studentIds = allUsers
            .Where(u => u.Hotel == hotel && u.Role == "Elev")
            .Select(u => u.Id)
            .ToList();
        
        return await _studentPlan.Find(p => studentIds.Contains(p.StudentId)).ToListAsync();
    }
    
    // Returnerer: En elevplan baseret på ID, eller null hvis den ikke findes.
    // Parametre: id – Planens ID.
    // Formål: Bruges til at hente en specifik plan.
    public async Task<StudentPlan?> GetPlanByIdAsync(int id)
    {
        return await _studentPlan.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    // Parametre: updatedPlan – Den opdaterede plan.
    // Formål: Opdaterer en eksisterende elevplan og fjerner notifikationer, hvis mål er fuldført.
    public async Task UpdateStudentPlanAsync(StudentPlan updatedPlan)
    {
        var existingPlan = await _studentPlan.Find(p => p.Id == updatedPlan.Id).FirstOrDefaultAsync();

        if (existingPlan != null)
        {
            foreach (var updatedGoal in updatedPlan.Goals)
            {
                var previousGoal = existingPlan.Goals.FirstOrDefault(g => g.Id == updatedGoal.Id);

                // Hvis status er ændret til "Fuldført"
                if (previousGoal != null &&
                    previousGoal.Status != "Fuldført" &&
                    updatedGoal.Status == "Fuldført")
                {
                    await _notificationRepo.RemoveGoalNotificationForAllUsersAsync(updatedPlan.Id, updatedGoal.Id); // Kalder RemoveGoal... metode i NotificationRepository.
                }
            }
        }

        await _studentPlan.ReplaceOneAsync(p => p.Id == updatedPlan.Id, updatedPlan); // Udskift hele planen med den opdaterede version
    }
    
    // Returnerer: True hvis planen blev slettet, ellers false.
    // Parametre: id – ID på planen der skal slettes.
    // Formål: Bruges til at fjerne en hel elevplan fra databasen.
    public async Task<bool> DeleteStudentPlanAsync(int id)
    {
        var result = await _studentPlan.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }
    
    // Returnerer: Deadline som DateTime.
    // Parametre: periodStart – Startdato for perioden, goalTitle – målets titel, planTitle – planens titel.
    // Formål: Beregner en passende deadline for et mål baseret på planens type og målets navn.
    private DateTime CalculateDeadline(DateTime periodStart, string goalTitle, string planTitle)
    {
        if (planTitle.Contains("1. praktik"))
        {
            if (goalTitle.Contains("inden første dag")) return periodStart.AddDays(-1);
            if (goalTitle.Contains("velkommen")) return periodStart.AddDays(7);
            if (goalTitle.Contains("information")) return periodStart.AddDays(14);
            if (goalTitle.Contains("sikkerhed")) return periodStart.AddMonths(1);
            if (goalTitle.Contains("samtaler")) return periodStart.AddDays(42);
            if (goalTitle.Contains("kurser")) return periodStart.AddMonths(2);
            if (goalTitle.Contains("faglige mål")) return periodStart.AddMonths(3);
            if (goalTitle.Contains("madspild")) return periodStart.AddMonths(3);
        }

        if (planTitle.Contains("2. praktik"))
        {
            if (goalTitle.Contains("evaluering")) return periodStart.AddDays(-1);
            if (goalTitle.Contains("interne")) return periodStart.AddDays(43 * 7);
            if (goalTitle.Contains("faglige mål")) return periodStart.AddDays(43 * 7);
        }

        if (planTitle.Contains("3. praktik"))
        {
            if (goalTitle.Contains("evaluering")) return periodStart.AddDays(-1);
            if (goalTitle.Contains("interne")) return periodStart.AddDays(43 * 7);
            if (goalTitle.Contains("faglige mål")) return periodStart.AddDays(43 * 7);
            if (goalTitle.Contains("klar-parat")) return periodStart.AddDays(43 * 7); // eller evt. -14
        }

        if (planTitle.Contains("afslutning"))
        {
            if (goalTitle.Contains("evaluering")) return periodStart.AddDays(28);
            if (goalTitle.Contains("interne")) return periodStart.AddDays(28);
        }

        return periodStart;
    }
}

    