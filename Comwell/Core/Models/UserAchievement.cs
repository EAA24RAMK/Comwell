namespace Core.Models;

public class UserAchievement
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int AchievementId { get; set; }
    public DateTime EarnedAt { get; set; }
    public bool IsVisible { get; set; } = true; // Om achievement skal vises på profil
} 