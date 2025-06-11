namespace Core.Models;

public class Quiz
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public int TimeLimitMinutes { get; set; } // I minutter
    public string Difficulty { get; set; } = "Medium"; // Easy, Medium, Hard
    public List<QuizQuestion> Questions { get; set; } = new();
} 