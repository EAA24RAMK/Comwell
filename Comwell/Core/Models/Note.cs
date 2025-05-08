namespace Core.Models;

public class Note
{
    public string AuthorId { get; set; } = "";
    public string Text { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}