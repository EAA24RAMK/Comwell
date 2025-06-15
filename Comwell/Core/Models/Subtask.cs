namespace Core.Models;

public class Subtask
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public string Status { get; set; } = "Ikke startet";
    public bool IsRequestedCompleted { get; set; } 
}