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

    public async Task<List<Notification>> GetNotificationsByUserAsync(int userId)
    {
        return await _http.GetFromJsonAsync<List<Notification>>($"api/notification/user/{userId}") ?? new();
    }

    public async Task DeleteNotificationForUserAsync(int notificationId, int userId)
    {
        var payload = new { NotificationId = notificationId, UserId = userId };
        await _http.PostAsJsonAsync("api/notification/delete-for-user", payload);
    }
}