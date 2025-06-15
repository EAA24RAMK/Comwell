using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Core.Models;
using Blazored.LocalStorage;

namespace WebApp.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly IUserService _userService;

    public CustomAuthenticationStateProvider(ILocalStorageService localStorage, IUserService userService)
    {
        _localStorage = localStorage;
        _userService = userService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var user = await _localStorage.GetItemAsync<User>("loggedInUser");
        
        if (user == null)
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("UserId", user.Id.ToString()),
            new Claim("Hotel", user.Hotel ?? ""),
            new Claim("SchoolType", user.SchoolType ?? ""),
            new Claim("Status", user.Status)
        };

        var identity = new ClaimsIdentity(claims, "localStorage");
        var principal = new ClaimsPrincipal(identity);

        return new AuthenticationState(principal);
    }

    public async Task MarkUserAsAuthenticated(User user)
    {
        await _localStorage.SetItemAsync("loggedInUser", user);
        
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("UserId", user.Id.ToString()),
            new Claim("Hotel", user.Hotel ?? ""),
            new Claim("SchoolType", user.SchoolType ?? ""),
            new Claim("Status", user.Status)
        };

        var identity = new ClaimsIdentity(claims, "localStorage");
        var principal = new ClaimsPrincipal(identity);

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(principal)));
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _localStorage.RemoveItemAsync("loggedInUser");
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
    }
} 