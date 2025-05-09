using Core.Models;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.JSInterop;

namespace WebApp.Services;

public class UserService : IUserService
{
    private readonly HttpClient _http;
    private readonly IJSRuntime _js;

    public UserService(HttpClient http, IJSRuntime js)
    {
        _http = http;
        _js = js;
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
    
    // Skal kigges igennem, måske kan den laves anderledes
    public async Task<User?> GetCurrentUserAsync()
    {
        var userJson = await _js.InvokeAsync<string>("localStorage.getItem", "loggedInUser");

        return string.IsNullOrEmpty(userJson)
            ? null
            : JsonSerializer.Deserialize<User>(userJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var response = await _http.DeleteAsync($"api/user/{userId}");
        return response.IsSuccessStatusCode;
    }
}