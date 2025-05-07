using Core.Models;
using System.Net.Http.Json;

namespace WebApp.Services;

public class UserService : IUserService
{
    private readonly HttpClient _http;

    public UserService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _http.GetFromJsonAsync<List<User>>("api/user") ?? new();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _http.GetFromJsonAsync<User>($"api/user/email/{email}");
    }

    public async Task<User?> CreateUserAsync(User newUser)
    {
        var response = await _http.PostAsJsonAsync("api/user", newUser);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<User>();
        return null;
    }

    public async Task<List<User>> GetUsersByRoleAsync(string role)
    {
        return await _http.GetFromJsonAsync<List<User>>($"api/user/role/{role}") ?? new();
    }

    public async Task<List<User>> GetUsersByHotelAsync(string hotel)
    {
        return await _http.GetFromJsonAsync<List<User>>($"api/user/hotel/{hotel}") ?? new();
    }
}