using Core.Models;
using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace WebApp.Services;

// Denne service bruges i frontend til at hente, oprette, slette og opdatere brugere.
// Den kommunikerer med backendens API og lokal storage i browseren.
// Bruges fx i UserPage.
public class UserService : IUserService
{
    // Instansvariabel til at sende HTTP-anmodninger til backendens API.
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;

    public UserService(HttpClient http, ILocalStorageService localStorage)
    {
        _http = http;
        _localStorage = localStorage;
    }

    // Returnerer: En liste med alle brugere fra backendens API.
    // Formål: Bruges til at vise alle brugere, fx i UserPage og ReportsPage.
    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _http.GetFromJsonAsync<List<User>>("api/user") ?? new(); // ?? betyder hvis noget går galt og metoden returnerer null, bliver en tom liste returneret.
    }

    // Returnerer: Den nyoprettede bruger eller null hvis noget gik galt.
    // Parametre: newUser – det brugerobjekt der skal oprettes.
    // Formål: Bruges når man opretter en ny bruger fra frontend (tilføj bruger).
    public async Task<User?> CreateUserAsync(User newUser)
    {
        var response = await _http.PostAsJsonAsync("api/user", newUser);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<User>(); // Oversætter JSON til user-objekt
        return null;
    }
    
    public async Task<User?> GetCurrentUserAsync()
    {
        return await _localStorage.GetItemAsync<User>("loggedInUser");
    }

    // Returnerer: True hvis brugeren blev slettet, ellers false.
    // Parametre: userId – ID på den bruger der skal slettes.
    // Formål: Bruges når en bruger fjernes fra systemet via frontend, fx i admin-panelet.
    public async Task<bool> DeleteUserAsync(int userId)
    {
        var response = await _http.DeleteAsync($"api/user/{userId}");
        return response.IsSuccessStatusCode;
    }
    
    // Returnerer: Brugerobjektet med det angivne ID, eller null hvis ikke fundet.
    // Parametre: id – ID på brugeren.
    // Formål: Bruges til at hente detaljer om én specifik bruger, fx ved redigering.
    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<User>($"api/user/id/{id}");
    }
    
    // Returnerer: True hvis status blev opdateret, ellers false.
    // Parametre:
    //   userId – ID på brugeren der skal opdateres
    //   newStatus – Ny statusværdi, fx "Aktiv", "Inaktiv"
    // Formål: Bruges til at ændre status på brugere i systemet, fx hvis de går på orlov.
    public async Task<bool> UpdateUserStatusAsync(int userId, string newStatus)
    {
        var response = await _http.PutAsJsonAsync($"api/user/{userId}/status", newStatus);
        return response.IsSuccessStatusCode;
    }

}