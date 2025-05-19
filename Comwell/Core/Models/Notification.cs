namespace Core.Models;

public class Notification
{
    public int Id { get; set; } 
    public string Message { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime Deadline { get; set; } 
    public int RelatedUserId { get; set; } 
    public int? PlanId { get; set; } 
    public int? GoalId { get; set; } 
    public bool IsRead { get; set; } = false;
    public List<int> NotifyUserId { get; set; } = new List<int>();
}