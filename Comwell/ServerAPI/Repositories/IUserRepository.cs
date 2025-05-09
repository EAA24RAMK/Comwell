using Core.Models;

namespace ServerAPI.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<List<User>> GetAllAsync();
    Task<User?> CreateAsync(User user);
    Task<List<User>> GetByRoleAsync(string role);
    Task<List<User>> GetByHotelAsync(string hotel);
    Task<bool> DeleteAsync(string id);
}