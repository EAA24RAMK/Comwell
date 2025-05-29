using Core.Models;

namespace ServerAPI.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<List<User>> GetAllAsync();
    Task<User?> CreateAsync(User user);
    Task<bool> DeleteAsync(int id);
    Task<User?> GetUserByIdAsync(int id);
    Task<bool> UpdateStatusAsync(int id, string newStatus);

}