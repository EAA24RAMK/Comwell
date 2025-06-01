namespace Core.Models;

public class Template // Bruges som reference til StudentPlan
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public List<Goal> Goals { get; set; } = new(); // Embedded liste af Goals
}