namespace Core.Models;

public class Subtask
{
    public string Text { get; set; } = "";
    public bool IsCompleted { get; set; }
    public bool IsRequestedCompleted { get; set; } 
}