using Core.Models;
using MongoDB.Driver;

namespace ServerAPI.Repositories;

// Repository overblik
// - Bruges til at gemme og hente opslag fra MongoDB.
// - Arbejder også med brugere for at filtrere opslag baseret på målgrupper.
// - Repositoryet bliver brugt til vores PostPage.
public class PostRepository : IPostRepository
{
    private readonly IMongoCollection<Post> _collection; // Instansvariabel for Post-collection i MongoDB
    private readonly IMongoCollection<User> _userCollection; // Instansvariabel for User-collection i MongoDB

    // Konstruktøren sørger for at etablere forbindelsen til MongoDB.
    // connection string og databasenavn læses fra appsettings.json.
    // Opretter adgang til både post og user, så vi kan oprette og hente opslag og målrette opslag til brugere.
    public PostRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _collection = db.GetCollection<Post>("post");
        _userCollection = db.GetCollection<User>("user");
    }
    
    // Parametre: post – Det opslag der skal oprettes.
    // Formål: Opretter et nyt opslag i databasen.
    // Tildeler et unikt ID (max eksisterende ID + 1) og sætter oprettelsesdato.
    public async Task<Post?> Create(Post post)
    {
        int maxId = 0;
        var allPosts = await _collection.Find(_ => true).ToListAsync();
        if (allPosts.Any())
        {
            maxId = allPosts.Max(t => t.Id); // Finder det højeste eksisterende ID
        }

        post.Id = maxId + 1; // Tildeler nyt ID
        
        post.CreatedAt = DateTime.UtcNow;
        await _collection.InsertOneAsync(post); // Gemmer brugeren i databasen
        return post;
    }

    public void Delete(int id)
    {
        _collection.DeleteOne(p => p.Id == id);
    }

    public List<Post> GetAll()
    {
        return _collection.Find(_ => true).ToList();
    }

    public List<Post> GetForUser(string username, string role)
    {
        if (role == "HR" || role == "Køkkenchef")
        {
            // Disse roller må se alt
            return _collection.Find(_ => true).ToList();
        }

        // Find brugerens ID
        var user = _userCollection.Find(u => u.Email == username).FirstOrDefault();
        if (user == null) return new();

        // Find opslag der er målrettet denne bruger eller ikke har nogen modtagere
        return _collection.Find(post =>
            post.TargetUserIds.Count == 0 || post.TargetUserIds.Contains(user.Id)).ToList();
    }
}