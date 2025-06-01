using Core.Models;
using System.Net.Http.Json;

namespace WebApp.Services;

// Service overblik
// - Bruges i frontend til at kommunikere med PostsController i backend.
// - Sender HTTP-requests for at oprette, hente og slette opslag.
// - Bruges i fx opslagsside.
public class PostService : IPostService
{
    private readonly HttpClient _http;  // Instansvariabel til at sende HTTP-anmodninger til backendens API

    // Konstruktør hvor HttpClient bliver injected, så servicen kan sende requests til backend.
    public PostService(HttpClient http)
    {
        _http = http;
    }

    // Returnerer: En liste af alle opslag fra API'et.
    // Formål: Henter alle opslag fra backend uden filtrering.
    public async Task<List<Post>> GetAllPostsAsync()
    {
        return await _http.GetFromJsonAsync<List<Post>>("api/posts") ?? new();
    }

    // Returnerer: Det oprettede opslag (eller tomt opslag hvis noget gik galt).
    // Parametre: post – Det opslag der skal oprettes.
    // Formål: Sender en POST-anmodning for at oprette et nyt opslag i backend.
    public async Task<Post> CreatePostAsync(Post post)
    {
        var response = await _http.PostAsJsonAsync("api/posts", post);
        return await response.Content.ReadFromJsonAsync<Post>() ?? new Post();
    }
    
    // Parametre: id – ID på opslaget der skal slettes.
    // Formål: Sender en DELETE-anmodning for at slette et opslag i backend.
    public async Task DeletePostAsync(int id)
    {
        await _http.DeleteAsync($"api/posts/{id}");
    }
    
    // Returnerer: En liste af opslag målrettet en bestemt bruger.
    // Parametre:
    //   email – Brugerens e-mail (bruges til at identificere bruger i backend).
    //   role – Brugerens rolle (fx "HR", "Køkkenchef", "Elev").
    // Formål: Henter opslag der er relevante for den specifikke bruger:
    // - HR og Køkkenchef får alle opslag.
    // - Elever og andre får opslag målrettet dem selv eller opslag uden specifik målgruppe.
    public async Task<List<Post>> GetPostsForUserAsync(string email, string role)
    {
        return await _http.GetFromJsonAsync<List<Post>>($"api/posts/mine?username={email}&role={role}") ?? new();
    }
}
