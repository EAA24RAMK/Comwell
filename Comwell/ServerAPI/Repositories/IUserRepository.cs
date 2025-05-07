using Core.Models;
using System.Threading.Tasks;

namespace ServerAPI.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
}