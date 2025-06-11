namespace Core.Models;

public class QuizAttemptAnswer
{
    public int Id { get; set; }
    public int AttemptId { get; set; }
    public int QuestionId { get; set; }
    public int? SelectedAnswerId { get; set; } // For multiple choice/true-false
    public string? TextAnswer { get; set; } // For text questions
    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }
} 