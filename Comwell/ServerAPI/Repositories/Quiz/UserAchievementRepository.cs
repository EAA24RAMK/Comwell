using Core.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace ServerAPI.Repositories;

public class UserAchievementRepository : IUserAchievementRepository
{
    private readonly IMongoCollection<UserAchievement> _userAchievements;

    public UserAchievementRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _userAchievements = db.GetCollection<UserAchievement>("userAchievement");
    }

    public async Task<List<UserAchievement>> GetAllAsync()
    {
        return await _userAchievements.Find(_ => true).ToListAsync();
    }

    public async Task<UserAchievement?> GetByIdAsync(int id)
    {
        return await _userAchievements.Find(ua => ua.Id == id).FirstOrDefaultAsync();
    }

    public async Task<UserAchievement?> CreateAsync(UserAchievement userAchievement)
    {
        int maxId = 0;
        var allUserAchievements = await _userAchievements.Find(_ => true).ToListAsync();
        if (allUserAchievements.Any())
        {
            maxId = allUserAchievements.Max(ua => ua.Id);
        }

        userAchievement.Id = maxId + 1;
        userAchievement.EarnedAt = DateTime.Now;
        
        await _userAchievements.InsertOneAsync(userAchievement);
        return userAchievement;
    }

    public async Task<List<UserAchievement>> GetByUserIdAsync(int userId)
    {
        return await _userAchievements.Find(ua => ua.UserId == userId).ToListAsync();
    }

    public async Task<bool> HasUserAchievementAsync(int userId, int achievementId)
    {
        var count = await _userAchievements.CountDocumentsAsync(ua => 
            ua.UserId == userId && ua.AchievementId == achievementId);
        return count > 0;
    }
} 