using Core.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace ServerAPI.Repositories;

public class AchievementRepository : IAchievementRepository
{
    private readonly IMongoCollection<Achievement> _achievements;

    public AchievementRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _achievements = db.GetCollection<Achievement>("achievement");
    }

    public async Task<List<Achievement>> GetAllAsync()
    {
        return await _achievements.Find(_ => true).ToListAsync();
    }

    public async Task<Achievement?> GetByIdAsync(int id)
    {
        return await _achievements.Find(a => a.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Achievement?> CreateAsync(Achievement achievement)
    {
        int maxId = 0;
        var allAchievements = await _achievements.Find(_ => true).ToListAsync();
        if (allAchievements.Any())
        {
            maxId = allAchievements.Max(a => a.Id);
        }

        achievement.Id = maxId + 1;
        await _achievements.InsertOneAsync(achievement);
        return achievement;
    }

    public async Task<Achievement?> UpdateAsync(Achievement achievement)
    {
        var filter = Builders<Achievement>.Filter.Eq(a => a.Id, achievement.Id);
        var result = await _achievements.ReplaceOneAsync(filter, achievement);
        return result.ModifiedCount > 0 ? achievement : null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var filter = Builders<Achievement>.Filter.Eq(a => a.Id, id);
        var update = Builders<Achievement>.Update.Set(a => a.IsActive, false);
        var result = await _achievements.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<List<Achievement>> GetActiveAchievementsAsync()
    {
        return await _achievements.Find(a => a.IsActive).ToListAsync();
    }
} 