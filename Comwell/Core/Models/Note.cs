namespace Core.Models;

public class Note // Embeddet i StudentPlan
{
    public int Id { get; set; }
    public string Text { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}