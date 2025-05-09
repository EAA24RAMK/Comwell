namespace Core.Models;

public class Goal
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public string Status { get; set; } = ""; // ikke påbegyndt, i gang, fuldført
}