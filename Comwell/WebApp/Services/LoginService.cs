using Core.Models;
using System.Net.Http.Json;

namespace WebApp.Services;

public class LoginService : ILoginService
{
    private readonly HttpClient _http;

    public LoginService(HttpClient http)
    {
        _http = http;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var response = await _http.PostAsJsonAsync("api/auth/login", new
        {
            email,
            password
        });

        if (response.IsSuccessStatusCode)
        {
            var user = await response.Content.ReadFromJsonAsync<User>();
            return user;
        }

        return null;
    }
}