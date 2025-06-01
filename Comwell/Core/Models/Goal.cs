namespace Core.Models;

public class Goal // embeddet i StudentPlan og Template
{
    public int Id { get; set; } 
    public string Title { get; set; } = "";
    public string Category { get; set; } = "";
    public DateTime Deadline { get; set; }
    public string Status { get; set; } = "";
    public string InitiatedBy { get; set; } = "";
    public bool CheckedOff { get; set; } = false;
    public List<Subtask> Subtasks { get; set; } = new(); // Embedded liste af delmål (Subtask)
}