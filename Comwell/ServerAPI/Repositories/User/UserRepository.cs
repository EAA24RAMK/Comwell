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

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _users.Find(u => u.Email == email).FirstOrDefaultAsync();
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _users.Find(_ => true).ToListAsync();
    }

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

    public async Task<List<User>> GetByRoleAsync(string role)
    {
        return await _users.Find(u => u.Role == role).ToListAsync();
    }

    public async Task<List<User>> GetByHotelAsync(string hotel)
    {
        return await _users.Find(u => u.Hotel == hotel).ToListAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _users.DeleteOneAsync(u => u.Id == id);
        return result.DeletedCount > 0;
    }
}