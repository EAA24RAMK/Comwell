using System.Net.Http.Json;
using Core.Models;

namespace WebApp.Services;

// Service overblik
// - Bruges i frontend til at hente og slette notifikationer for en bruger.
// - Kommunikerer med NotificationController i backend via HTTP-requests.
public class NotificationService : INotificationService
{
    // Instansvariabel til at sende HTTP-anmodninger til backendens API
    private readonly HttpClient _http;
    
    // Konstruktør hvor HttpClient bliver injected, så servicen kan sende requests.
    public NotificationService(HttpClient http)
    {
        _http = http;
    }

    // Returnerer: En liste af notifikationer for en bestemt bruger.
    // Parametre: userId – ID på brugeren vi vil hente notifikationer for.
    // Formål: Sender en GET-anmodning til backend for at hente alle aktive notifikationer for brugeren.
    public async Task<List<Notification>> GetNotificationsByUserAsync(int userId)
    {
        return await _http.GetFromJsonAsync<List<Notification>>($"api/notification/user/{userId}") ?? new();
    }
    
    // Parametre:
    //   notificationId – ID på notifikationen der skal slettes.
    //   userId – ID på brugeren der sletter notifikationen.
    // Formål: Sender en POST-anmodning til backend for at markere en notifikation som slettet for brugeren.
    public async Task DeleteNotificationForUserAsync(int notificationId, int userId)
    {
        await _http.PostAsync($"api/notification/{notificationId}/delete-for-user/{userId}", null);
    }
}