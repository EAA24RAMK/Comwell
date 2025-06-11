using Microsoft.AspNetCore.Mvc;
using Core.Models;
using ServerAPI.Repositories;

namespace ServerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizController : ControllerBase
{
    private readonly IQuizRepository _quizRepo;
    private readonly IQuizAttemptRepository _attemptRepo;
    private readonly IAchievementRepository _achievementRepo;
    private readonly IUserAchievementRepository _userAchievementRepo;

    public QuizController(
        IQuizRepository quizRepo,
        IQuizAttemptRepository attemptRepo,
        IAchievementRepository achievementRepo,
        IUserAchievementRepository userAchievementRepo)
    {
        _quizRepo = quizRepo;
        _attemptRepo = attemptRepo;
        _achievementRepo = achievementRepo;
        _userAchievementRepo = userAchievementRepo;
    }

    // Get all active quizzes
    [HttpGet]
    public async Task<ActionResult<List<Quiz>>> GetAllQuizzes()
    {
        return Ok(await _quizRepo.GetActiveQuizzesAsync());
    }

    // Get quiz by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Quiz>> GetQuizById(int id)
    {
        var quiz = await _quizRepo.GetByIdAsync(id);
        if (quiz == null) return NotFound();
        return Ok(quiz);
    }

    // Create new quiz
    [HttpPost]
    public async Task<ActionResult<Quiz>> CreateQuiz(Quiz quiz)
    {
        var created = await _quizRepo.CreateAsync(quiz);
        return Ok(created);
    }

    // Update quiz
    [HttpPut("{id}")]
    public async Task<ActionResult<Quiz>> UpdateQuiz(int id, Quiz quiz)
    {
        quiz.Id = id;
        var updated = await _quizRepo.UpdateAsync(quiz);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    // Delete quiz (soft delete)
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuiz(int id)
    {
        var deleted = await _quizRepo.DeleteAsync(id);
        if (!deleted) return NotFound();
        return Ok();
    }

    // Start a new quiz attempt
    [HttpPost("start")]
    public async Task<ActionResult<QuizAttempt>> StartQuiz([FromBody] StartQuizRequest request)
    {
        var quiz = await _quizRepo.GetByIdAsync(request.QuizId);
        if (quiz == null) return NotFound("Quiz not found");

        var attempt = new QuizAttempt
        {
            QuizId = request.QuizId,
            UserId = request.UserId,
            MaxScore = quiz.Questions.Sum(q => q.Points)
        };

        var created = await _attemptRepo.CreateAsync(attempt);
        return Ok(created);
    }

    // Submit quiz attempt
    [HttpPost("submit")]
    public async Task<ActionResult<QuizAttempt>> SubmitQuiz([FromBody] QuizAttempt attempt)
    {
        var quiz = await _quizRepo.GetByIdAsync(attempt.QuizId);
        if (quiz == null) return NotFound("Quiz not found");

        // Calculate score
        int totalScore = 0;
        foreach (var userAnswer in attempt.UserAnswers)
        {
            var question = quiz.Questions.FirstOrDefault(q => q.Id == userAnswer.QuestionId);
            if (question != null)
            {
                if (question.QuestionType == "MultipleChoice" || question.QuestionType == "TrueFalse")
                {
                    var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect);
                    if (correctAnswer != null && userAnswer.SelectedAnswerId == correctAnswer.Id)
                    {
                        userAnswer.IsCorrect = true;
                        userAnswer.PointsEarned = question.Points;
                        totalScore += question.Points;
                    }
                }
            }
        }

        attempt.Score = totalScore;
        attempt.Percentage = attempt.MaxScore > 0 ? (double)totalScore / attempt.MaxScore * 100 : 0;
        attempt.CompletedAt = DateTime.Now;
        attempt.IsCompleted = true;

        var updated = await _attemptRepo.UpdateAsync(attempt);
        
        // Check and award achievements
        await CheckAndAwardAchievements(attempt.UserId);

        return Ok(updated);
    }

    // Get user attempts
    [HttpGet("attempts/user/{userId}")]
    public async Task<ActionResult<List<QuizAttempt>>> GetUserAttempts(int userId)
    {
        var attempts = await _attemptRepo.GetByUserIdAsync(userId);
        return Ok(attempts.Where(a => a.IsCompleted).ToList());
    }

    // Get quiz attempts
    [HttpGet("attempts/quiz/{quizId}")]
    public async Task<ActionResult<List<QuizAttempt>>> GetQuizAttempts(int quizId)
    {
        var attempts = await _attemptRepo.GetByQuizIdAsync(quizId);
        return Ok(attempts.Where(a => a.IsCompleted).ToList());
    }

    // Get leaderboard
    [HttpGet("leaderboard")]
    public async Task<ActionResult<List<QuizAttempt>>> GetLeaderboard([FromQuery] int? quizId = null)
    {
        return Ok(await _attemptRepo.GetLeaderboardAsync(quizId));
    }

    // Get all achievements
    [HttpGet("achievements")]
    public async Task<ActionResult<List<Achievement>>> GetAllAchievements()
    {
        return Ok(await _achievementRepo.GetActiveAchievementsAsync());
    }

    // Get user achievements
    [HttpGet("achievements/user/{userId}")]
    public async Task<ActionResult<List<UserAchievement>>> GetUserAchievements(int userId)
    {
        return Ok(await _userAchievementRepo.GetByUserIdAsync(userId));
    }

    private async Task CheckAndAwardAchievements(int userId)
    {
        var userAttempts = await _attemptRepo.GetByUserIdAsync(userId);
        var completedAttempts = userAttempts.Where(a => a.IsCompleted).ToList();

        // First Quiz Completion
        if (completedAttempts.Any() && !await _userAchievementRepo.HasUserAchievementAsync(userId, 1))
        {
            await _userAchievementRepo.CreateAsync(new UserAchievement { UserId = userId, AchievementId = 1 });
        }

        // Perfect Score
        if (completedAttempts.Any(a => a.Percentage >= 100) && !await _userAchievementRepo.HasUserAchievementAsync(userId, 2))
        {
            await _userAchievementRepo.CreateAsync(new UserAchievement { UserId = userId, AchievementId = 2 });
        }

        // Quiz Master (5+ completed quizzes)
        if (completedAttempts.Count >= 5 && !await _userAchievementRepo.HasUserAchievementAsync(userId, 3))
        {
            await _userAchievementRepo.CreateAsync(new UserAchievement { UserId = userId, AchievementId = 3 });
        }

        // Scholar (high average score)
        var averageScore = completedAttempts.Any() ? completedAttempts.Average(a => a.Percentage) : 0;
        if (averageScore >= 85 && completedAttempts.Count >= 3 && !await _userAchievementRepo.HasUserAchievementAsync(userId, 5))
        {
            await _userAchievementRepo.CreateAsync(new UserAchievement { UserId = userId, AchievementId = 5 });
        }
    }
}

public class StartQuizRequest
{
    public int QuizId { get; set; }
    public int UserId { get; set; }
} 