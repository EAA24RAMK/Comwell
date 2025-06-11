namespace Core.Models;

public class Achievement
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Icon { get; set; } = "🏆"; // Emoji eller CSS klasse
    public string Category { get; set; } = "Quiz"; // Quiz, Performance, etc.
    public string Requirements { get; set; } = ""; // Beskrivelse af krav
    public bool IsActive { get; set; } = true;
} 