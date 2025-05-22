namespace Core.Models;

public class User // Referencing, bruges på tværs af hele systemet
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string Role { get; set; }
    public string Hotel { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? SchoolType { get; set; }
}