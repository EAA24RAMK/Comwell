using Core.Models;

namespace WebApp.Services;

public interface IUserService
{
    Task<List<User>> GetAllUsersAsync();
    Task<User?> GetUserByEmailAsync(string email);
    Task<User> CreateUserAsync(User user);
    Task<List<User>> GetUsersByRoleAsync(string role);
    Task<List<User>> GetUsersByHotelAsync(string hotel);
    
    // Henter den aktuelle bruger fra localstorage
    Task<User?> GetCurrentUserAsync();
    Task<bool> DeleteUserAsync(int userId);
}