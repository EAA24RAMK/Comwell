namespace Core.Models;

public class LearningMaterial
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int SubtaskId { get; set; }
    public bool IsLink { get; set; }
    public string? LinkUrl { get; set; }
    public string? FileName { get; set; }
    public string? FileId { get; set; } // GridFS file ID
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}