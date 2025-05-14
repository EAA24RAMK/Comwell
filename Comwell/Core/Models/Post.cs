namespace Core.Models;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string Author { get; set; } = "";
    public List<string> VisibleTo { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}