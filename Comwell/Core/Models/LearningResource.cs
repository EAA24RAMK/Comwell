namespace Core.Models;

public class LearningResource
{
    public int Id { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public string Url { get; set; }
    public string LinkedGoalId { get; set; }
    public int DurationMinutes { get; set; }
    public int UnlockLevel { get; set; }

    public Quiz? Quiz { get; set; } // Indlejret quiz
}   