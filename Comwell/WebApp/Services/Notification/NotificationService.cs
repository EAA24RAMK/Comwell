using System.Net.Http.Json;
using Core.Models;

namespace WebApp.Services;

public class NotificationService : INotificationService
{
    private readonly HttpClient _http;
    public NotificationService(HttpClient http)
    {
        _http = http;
    }
    
    public async Task<List<Notification>> GetAllNotificationsAsync()
    {
        return await _http.GetFromJsonAsync<List<Notification>>("api/notification") ?? new();
    }

    public async Task<List<Notification>> GetNotificationsByUserAsync(int userId)
    {
        return await _http.GetFromJsonAsync<List<Notification>>($"api/notification/user/{userId}") ?? new();
    }

    public async Task AddNotificationAsync(Notification notification)
    {
        await _http.PostAsJsonAsync("api/notification", notification);
    }

    public async Task UpdateNotificationAsync(Notification notification)
    {
        await _http.PutAsJsonAsync($"api/notification", notification);
    }

    public async Task DeleteNotificationAsync(int id)
    {
        await _http.DeleteAsync($"api/notification/{id}");
    }
}