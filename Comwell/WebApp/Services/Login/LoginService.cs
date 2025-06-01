using Core.Models;
using System.Net.Http.Json;

namespace WebApp.Services;

// Service overblik
// - Bruges til at logge brugere ind i systemet.
// - Sender login-oplysninger til backend og får brugerdata retur, hvis login lykkes.
public class LoginService : ILoginService
{
    private readonly HttpClient _http; // HTTP-klient der bruges til at sende requests til API'et

    // Konstruktør hvor HttpClient bliver injected, så servicen kan sende requests.
    public LoginService(HttpClient http)
    {
        _http = http;
    }

    // Returnerer: En User hvis login er succesfuldt, ellers null.
    // Parametre:
    //   email – Brugerens e-mail.
    //   password – Brugerens adgangskode.
    // Formål: Forsøger at logge brugeren ind ved at sende e-mail og kode til backend.
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