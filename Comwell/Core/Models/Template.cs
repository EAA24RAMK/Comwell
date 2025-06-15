namespace Core.Models;

public class Template // referencing, genbruges til mange planer
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public List<Goal> Goals { get; set; } = new();
}