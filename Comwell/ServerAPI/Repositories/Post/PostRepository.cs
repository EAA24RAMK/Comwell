using Core.Models;
using MongoDB.Driver;

namespace ServerAPI.Repositories;

public class PostRepository : IPostRepository
{
    private readonly IMongoCollection<Post> _collection;
    
    public PostRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _collection = db.GetCollection<Post>("post");
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
    
    // Slet et opslag
    public void Delete(int id)
    {
        _collection.DeleteOne(p => p.Id == id);
    }

    
    // Hent alle opslag
    public List<Post> GetAll()
    {
        return _collection.Find(_ => true).ToList();
    }
    
    public List<Post> GetForUser(string username, string role)
    {
        return _collection.Find(_ => true).ToList(); // Samme som GetAll()
    }
}