namespace Core.Models;

public class QuizAttempt
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public int UserId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int Score { get; set; }
    public int MaxScore { get; set; }
    public double Percentage { get; set; }
    public bool IsCompleted { get; set; } = false;
    public List<QuizAttemptAnswer> UserAnswers { get; set; } = new();
} 