using Core.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace ServerAPI.Repositories;

public class QuizRepository : IQuizRepository
{
    private readonly IMongoCollection<Quiz> _quizzes;

    public QuizRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _quizzes = db.GetCollection<Quiz>("quiz");
    }

    public async Task<List<Quiz>> GetAllAsync()
    {
        return await _quizzes.Find(_ => true).ToListAsync();
    }

    public async Task<Quiz?> GetByIdAsync(int id)
    {
        return await _quizzes.Find(q => q.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Quiz?> CreateAsync(Quiz quiz)
    {
        int maxId = 0;
        var allQuizzes = await _quizzes.Find(_ => true).ToListAsync();
        if (allQuizzes.Any())
        {
            maxId = allQuizzes.Max(q => q.Id);
        }

        quiz.Id = maxId + 1;
        quiz.CreatedAt = DateTime.Now;
        
        // Assign IDs to questions and answers
        int questionId = 1;
        foreach (var question in quiz.Questions)
        {
            question.Id = questionId++;
            question.QuizId = quiz.Id;
            
            int answerId = 1;
            foreach (var answer in question.Answers)
            {
                answer.Id = answerId++;
                answer.QuestionId = question.Id;
            }
        }

        await _quizzes.InsertOneAsync(quiz);
        return quiz;
    }

    public async Task<Quiz?> UpdateAsync(Quiz quiz)
    {
        var filter = Builders<Quiz>.Filter.Eq(q => q.Id, quiz.Id);
        var result = await _quizzes.ReplaceOneAsync(filter, quiz);
        return result.ModifiedCount > 0 ? quiz : null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var filter = Builders<Quiz>.Filter.Eq(q => q.Id, id);
        var update = Builders<Quiz>.Update.Set(q => q.IsActive, false);
        var result = await _quizzes.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<List<Quiz>> GetActiveQuizzesAsync()
    {
        return await _quizzes.Find(q => q.IsActive).ToListAsync();
    }
} 