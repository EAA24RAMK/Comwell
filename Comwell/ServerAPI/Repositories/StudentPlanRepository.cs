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
    
    public async Task CreateStudentPlanAsync(StudentPlan createPlan)
    {
        await _studentPlan.InsertOneAsync(createPlan);
    }
    
    public async Task<List<StudentPlan>> GetAllPlansAsync()
    {
        return await _studentPlan.Find(_ => true).ToListAsync();
    }

    public async Task<List<StudentPlan>> GetPlansByStudentAsync(string studentId)
    {
        return await _studentPlan.Find(p => p.StudentId == studentId).ToListAsync();
    }

    // Henter alle elevplaner fra et hotel, så køkkenchefen kan se sine elevers planer
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
    
}