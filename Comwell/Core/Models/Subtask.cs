namespace Core.Models;

public class Subtask
{
    public string Text { get; set; } = "";
    public string Status { get; set; } = "Ikke startet";
    public bool IsRequestedCompleted { get; set; } 
}