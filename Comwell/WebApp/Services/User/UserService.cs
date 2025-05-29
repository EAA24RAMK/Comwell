using Core.Models;
using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace WebApp.Services;

public class UserService : IUserService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public UserService(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
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
    
    public async Task<User?> GetCurrentUserAsync()
    {
        return await _localStorage.GetItemAsync<User>("loggedInUser");
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var response = await _http.DeleteAsync($"api/user/{userId}");
        return response.IsSuccessStatusCode;
    }
    
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<User>($"api/user/id/{id}");
    }
    
    public async Task<bool> UpdateUserStatusAsync(int userId, string newStatus)
    {
        var response = await _http.PutAsJsonAsync($"api/user/{userId}/status", newStatus);
        return response.IsSuccessStatusCode;
    }
}