using Core.Models;
using MongoDB.Driver; // Giver adgang til at arbejde med MongoDB-databasen

namespace ServerAPI.Repositories;

/* Repository overblik
- Er ansvarlig for alt, der har med brugerdata at gøre.
- Bindeled mellem databasen og resten af systemet.
- Metoder til at hente, oprette, opdatere og slette brugere.
- Repositoryet bliver brugt i vores UserController og AuthController. */
public class UserRepository : IUserRepository
{
    // Instansvariabel der referer til User-collection i MongoDB
    private readonly IMongoCollection<User> _users;

    // Konstruktøren sørger for at etablere forbindelsen til MongoDB.
    // connection string og databasenavn læses fra appsettings.json.
    public UserRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _users = db.GetCollection<User>("user");
    }

    // Returnerer: En bruger med den angivne e-mail.
    // Parametre: email – den e-mail vi søger efter.
    // Formål: Bruges i AuthController til login for at finde brugeren via e-mail.
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    // Returnerer: En liste med alle brugere i databasen.
    // Formål: Bruges fx til HR-overblik over alle brugere.
    public async Task<List<User>> GetAllAsync()
    {
        return await _users.Find(_ => true).ToListAsync();
    }

    // Returnerer: Den nyoprettede bruger med sat ID og oprettelsestid.
    // Parametre: user – et brugerobjekt uden ID.
    // Formål: Opretter en ny bruger ved at finde højeste eksisterende ID og lægge én til.
    public async Task<User?> CreateAsync(User user)
    {
        int maxId = 0;
        var allUsers = await _users.Find(_ => true).ToListAsync();
        if (allUsers.Any())
        {
            maxId = allUsers.Max(t => t.Id); // Finder det højeste eksisterende ID
        }

        user.Id = maxId + 1; // Tildeler nyt ID
        
        user.CreatedAt = DateTime.Now; // Registrerer oprettelsesdato
        await _users.InsertOneAsync(user); // Gemmer brugeren i databasen
        return user;
    }

    // Returnerer: True hvis brugeren blev slettet, ellers false (bool).
    // Parametre: id – brugerens ID.
    // Formål: Bruges når man vil fjerne en bruger helt fra systemet.
    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _users.DeleteOneAsync(u => u.Id == id);
        return result.DeletedCount > 0; 
    }
    
    // Returnerer: En bruger baseret på deres ID, eller null hvis den ikke findes.
    // Parametre: id – brugerens ID.
    // Formål: Bruges fx til profilvisning eller redigering.
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _users.Find(u => u.Id == id).FirstOrDefaultAsync();
    }

    // Returnerer: True hvis status blev opdateret, ellers false.
    // Parametre: 
    //   id – brugerens ID
    //   newStatus – den nye statusværdi, fx "Aktiv", "Inaktiv"
    // Formål: Bruges til at opdatere en brugers status uden at ændre andre oplysninger.
    public async Task<bool> UpdateStatusAsync(int id, string newStatus)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Id, id);
        var update = Builders<User>.Update.Set(u => u.Status, newStatus);

        var result = await _users.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }
}