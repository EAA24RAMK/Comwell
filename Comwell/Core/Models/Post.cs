namespace Core.Models;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Author { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public List<int> TargetUserIds { get; set; } = new(); // Reference til User (via id)
    public string? Url { get; set; }
}