namespace Core.Models;

public class Goal
{
    public int Id { get; set; } 
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public DateTime Deadline { get; set; }
    public string Status { get; set; } = "";
    public string Responsible { get; set; } = "";
    public string InitiatedBy { get; set; } = "";
    public bool CheckedOff { get; set; } = false;

    public List<string> Subtasks { get; set; } = new();
    public List<string> Comments { get; set; } = new();
    public List<string> StudentNotes { get; set; } = new();
    public List<string> Attachments { get; set; } = new();
}