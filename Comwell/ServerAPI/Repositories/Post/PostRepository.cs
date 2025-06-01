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
        await _collection.InsertOneAsync(post); // Gemmer opslag i databasen
        return post;
    }

    // Parametre: id – ID på opslaget der skal slettes.
    // Formål: Sletter et opslag fra databasen baseret på dets ID.
    public void Delete(int id)
    {
        _collection.DeleteOne(p => p.Id == id);
    }

    // Returnerer: Liste af alle opslag.
    // Formål: Henter alle opslag fra databasen.
    public List<Post> GetAll()
    {
        return _collection.Find(_ => true).ToList();
    }

    // Returnerer: Liste af opslag målrettet brugeren.
    // Parametre: 
    //   username – E-mail på brugeren der er logget ind.
    //   role – Brugerens rolle ("HR", "Køkkenchef", "Elev", "Kok".).
    // Formål: Henter opslag baseret på brugerens rolle:
    // - HR og Køkkenchef: Må se alle opslag.
    // - Andre (fx elev): Må se opslag målrettet dem eller opslag uden målgruppe.
    public List<Post> GetForUser(string username, string role)
    {
        if (role == "HR" || role == "Køkkenchef")
        {
            // Disse roller må se alt
            return _collection.Find(_ => true).ToList();
        }

        // Find bruger baseret på E-mail
        var user = _userCollection.Find(u => u.Email == username).FirstOrDefault();
        if (user == null) return new();

        // Find opslag der enten:
        // - Ikke har målgruppe (TargetUserIds.Count == 0), eller
        // - Har denne brugers ID i målgruppen
        return _collection.Find(post =>
            post.TargetUserIds.Count == 0 || post.TargetUserIds.Contains(user.Id)).ToList();
    }
}