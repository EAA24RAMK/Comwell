namespace Core.Models;

// Denne klasse beskriver en quiz, som indlejres i LearningResource
public class Quiz
{
    public string Question { get; set; }
    public List<string> Options { get; set; }
    public int CorrectAnswerIndex { get; set; }
}