using Core.Models;
using System.Threading.Tasks;

namespace WebApp.Services;

public interface ILoginService
{
    Task<User?> LoginAsync(string email, string password);
}