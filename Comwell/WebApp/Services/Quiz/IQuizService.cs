using Core.Models;

namespace WebApp.Services;

public interface IQuizService
{
    // Quiz management
    Task<List<Quiz>> GetAllQuizzesAsync();
    Task<Quiz?> GetQuizByIdAsync(int id);
    Task<Quiz> CreateQuizAsync(Quiz quiz);
    Task<Quiz> UpdateQuizAsync(Quiz quiz);
    Task DeleteQuizAsync(int id);
    
    // Quiz attempts
    Task<QuizAttempt> StartQuizAsync(int quizId, int userId);
    Task<QuizAttempt> SubmitQuizAsync(QuizAttempt attempt);
    Task<List<QuizAttempt>> GetUserAttemptsAsync(int userId);
    Task<List<QuizAttempt>> GetQuizAttemptsAsync(int quizId);
    
    // Leaderboard
    Task<List<QuizAttempt>> GetLeaderboardAsync(int? quizId = null);
    
    // Achievements
    Task<List<Achievement>> GetAllAchievementsAsync();
    Task<List<UserAchievement>> GetUserAchievementsAsync(int userId);
    Task CheckAndAwardAchievementsAsync(int userId);
} 