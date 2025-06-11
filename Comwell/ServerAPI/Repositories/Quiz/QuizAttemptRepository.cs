using Core.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace ServerAPI.Repositories;

public class QuizAttemptRepository : IQuizAttemptRepository
{
    private readonly IMongoCollection<QuizAttempt> _attempts;

    public QuizAttemptRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _attempts = db.GetCollection<QuizAttempt>("quizAttempt");
    }

    public async Task<List<QuizAttempt>> GetAllAsync()
    {
        return await _attempts.Find(_ => true).ToListAsync();
    }

    public async Task<QuizAttempt?> GetByIdAsync(int id)
    {
        return await _attempts.Find(a => a.Id == id).FirstOrDefaultAsync();
    }

    public async Task<QuizAttempt?> CreateAsync(QuizAttempt attempt)
    {
        int maxId = 0;
        var allAttempts = await _attempts.Find(_ => true).ToListAsync();
        if (allAttempts.Any())
        {
            maxId = allAttempts.Max(a => a.Id);
        }

        attempt.Id = maxId + 1;
        attempt.StartedAt = DateTime.Now;

        await _attempts.InsertOneAsync(attempt);
        return attempt;
    }

    public async Task<QuizAttempt?> UpdateAsync(QuizAttempt attempt)
    {
        var filter = Builders<QuizAttempt>.Filter.Eq(a => a.Id, attempt.Id);
        var result = await _attempts.ReplaceOneAsync(filter, attempt);
        return result.ModifiedCount > 0 ? attempt : null;
    }

    public async Task<List<QuizAttempt>> GetByUserIdAsync(int userId)
    {
        return await _attempts.Find(a => a.UserId == userId).ToListAsync();
    }

    public async Task<List<QuizAttempt>> GetByQuizIdAsync(int quizId)
    {
        return await _attempts.Find(a => a.QuizId == quizId).ToListAsync();
    }

    public async Task<List<QuizAttempt>> GetLeaderboardAsync(int? quizId = null, int limit = 10)
    {
        var filter = Builders<QuizAttempt>.Filter.Eq(a => a.IsCompleted, true);
        
        if (quizId.HasValue)
        {
            filter = filter & Builders<QuizAttempt>.Filter.Eq(a => a.QuizId, quizId.Value);
        }

        var sort = Builders<QuizAttempt>.Sort
            .Descending(a => a.Percentage)
            .Descending(a => a.Score)
            .Ascending(a => a.CompletedAt);

        return await _attempts.Find(filter)
            .Sort(sort)
            .Limit(limit)
            .ToListAsync();
    }

    public async Task<List<QuizAttempt>> GetCompletedAttemptsAsync()
    {
        return await _attempts.Find(a => a.IsCompleted).ToListAsync();
    }
} 