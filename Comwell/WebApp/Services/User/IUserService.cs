using Core.Models;

namespace WebApp.Services;

public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User> CreateUserAsync(User user);
    
    // Henter den aktuelle bruger fra localstorage
    Task<User?> GetCurrentUserAsync();
    Task<bool> DeleteUserAsync(int userId);
    Task<User?> GetUserByIdAsync(int id);
    Task<bool> UpdateUserStatusAsync(int userId, string newStatus);
}