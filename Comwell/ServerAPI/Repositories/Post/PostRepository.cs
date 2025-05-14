using Core.Models;
using MongoDB.Driver;

namespace ServerAPI.Repositories;

public class PostRepository : IPostRepository
{
    private readonly IMongoCollection<Post> _collection;
    
    public PostRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Post>("Posts");
    }
    
    // Opret nyt opslag med nyt ID
    public void Create(Post post)
    {
        var maxId = _collection.Find(_ => true)
            .SortByDescending(post => post.Id)
            .Limit(1)
            .FirstOrDefault()?.Id ?? 0;
        
        post.Id = maxId + 1;
        post.CreatedAt = DateTime.UtcNow;
        
        _collection.InsertOne(post);
    }
    
    // Hent alle opslag
    public List<Post> GetAll()
    {
        return _collection.Find(_ => true).ToList();
    }
    
    // Hent opslag synlige for en bestemt bruger/rolle
    public List<Post> GetForUser(string username, string role)
    {
        return _collection.Find(post =>
            post.VisibleTo.Contains("Alle") ||
            post.VisibleTo.Contains(role) ||
            post.VisibleTo.Contains(username)
        ).ToList();
    }
}