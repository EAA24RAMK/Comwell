namespace Core.Models;

public class Template
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string CreatedBy { get; set; } = "";
    public List<Goal> Goals { get; set; } = new();
}