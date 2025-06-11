using Core.Models;

namespace ServerAPI.Repositories;

public interface IQuizAttemptRepository
{
    Task<List<QuizAttempt>> GetAllAsync();
    Task<QuizAttempt?> GetByIdAsync(int id);
    Task<QuizAttempt?> CreateAsync(QuizAttempt attempt);
    Task<QuizAttempt?> UpdateAsync(QuizAttempt attempt);
    Task<List<QuizAttempt>> GetByUserIdAsync(int userId);
    Task<List<QuizAttempt>> GetByQuizIdAsync(int quizId);
    Task<List<QuizAttempt>> GetLeaderboardAsync(int? quizId = null, int limit = 10);
    Task<List<QuizAttempt>> GetCompletedAttemptsAsync();
} 