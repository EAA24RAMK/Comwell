namespace Core.Models;

public class QuizQuestion
{
    public int Id { get; set; }
    public int QuizId { get; set; }
    public string QuestionText { get; set; } = "";
    public string QuestionType { get; set; } = "MultipleChoice"; // MultipleChoice, TrueFalse, Text
    public int Points { get; set; } = 1;
    public List<QuizAnswer> Answers { get; set; } = new();
} 