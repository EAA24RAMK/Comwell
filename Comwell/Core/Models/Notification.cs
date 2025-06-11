namespace Core.Models;

public class Notification
{
    public int Id { get; set; } 
    public string Message { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime Deadline { get; set; } 
    public int? PlanId { get; set; } // Reference til StudentPlan
    public int? GoalId { get; set; } // Reference til Goal
    public bool IsRead { get; set; } = false;
    public string? Link { get; set; } // URL eller side man kan klikke sig videre til
    public List<int> DeletedByUserIds { get; set; } = new(); 
    public List<int> NotifyUserId { get; set; } = new List<int>();
}