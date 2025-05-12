using Core.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace ServerAPI.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _users = db.GetCollection<User>("user");
    }

    // Find brugere ved at søge på email
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    // Finder alle brugere og returnerer dem i en liste
    public async Task<List<User>> GetAllAsync()
    {
        return await _users.Find(_ => true).ToListAsync();
    }

    // Opret bruger: Finder højeste ID der er, lægger en til
    public async Task<User?> CreateAsync(User user)
    {
        int maxId = 0;
        var allUsers = await _users.Find(_ => true).ToListAsync();
        if (allUsers.Any())
        {
            maxId = allUsers.Max(t => t.Id);
        }

        user.Id = maxId + 1;
        
        user.CreatedAt = DateTime.Now;
        await _users.InsertOneAsync(user);
        return user;
    }

    // Returnerer en liste med alle brugernes roller
    public async Task<List<User>> GetByRoleAsync(string role)
    {
        return await _users.Find(u => u.Role == role).ToListAsync();
    }

    // Returnerer en liste med alle brugernes hoteller
    public async Task<List<User>> GetByHotelAsync(string hotel)
    {
        return await _users.Find(u => u.Hotel == hotel).ToListAsync();
    }

    // Sletter en bruger baseret på ID
    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _users.DeleteOneAsync(u => u.Id == id);
        return result.DeletedCount > 0;
    }
}