using System.Net.Http.Headers;
using System.Net.Http.Json;
using Core.Models;

namespace WebApp.Services;

// Service overblik
// - Bruges i frontend til at arbejde med læringsmaterialer via HTTP-requests.
// - Kan uploade filer, tilføje links, hente og slette læringsmaterialer.
public class LearningMaterialService : ILearningMaterialService
{
    private readonly HttpClient _http; // HTTP-klient til at sende requests til backend

    // Konstruktør hvor HttpClient bliver injected, så vi kan kalde API'et.
    public LearningMaterialService(HttpClient http)
    {
        _http = http;
    }

    // Returnerer: Det oprettede læringsmateriale eller null ved fejl.
    // Parametre:
    //   fileStream – Stream med indholdet af filen.
    //   fileName – Navn på filen.
    //   title – Titel på læringsmaterialet.
    //   subtaskId – ID på det delmål læringsmaterialet hører til.
    // Formål: Upload en fil til backend som læringsmateriale.
    public async Task<LearningMaterial?> UploadFileAsync(Stream fileStream, string fileName, string title, int subtaskId)
    {
        var content = new MultipartFormDataContent(); // HTTP-form data container

        var fileContent = new StreamContent(fileStream); // Stream indholdet af filen
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream"); // Sæt indholdstype

        // Tilføj fil og metadata til form data
        content.Add(fileContent, "file", fileName);
        content.Add(new StringContent(title), "title");
        content.Add(new StringContent(subtaskId.ToString()), "subtaskId");

        // Send POST-request til API'et med fil og metadata
        var response = await _http.PostAsync("api/LearningMaterial/upload", content);
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<LearningMaterial>();
    }

    // Returnerer: Det oprettede læringsmateriale-link eller null ved fejl.
    // Parametre: link – Læringsmateriale-objekt der indeholder linkinformation.
    // Formål: Opret et læringsmateriale baseret på et link (ikke en fil).
    public async Task<LearningMaterial?> AddLinkAsync(LearningMaterial link)
    {
        var response = await _http.PostAsJsonAsync("api/LearningMaterial/link", link);
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<LearningMaterial>();
    }

    // Returnerer: True hvis materialet blev slettet, ellers false.
    // Parametre: id – ID på læringsmaterialet der skal slettes.
    // Formål: Slet et læringsmateriale i backend (enten fil eller link).
    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/LearningMaterial/{id}");
        return response.IsSuccessStatusCode;
    }
    
    // Returnerer: Liste af alle læringsmaterialer (filer og links).
    // Formål: Hent alle læringsmaterialer fra backend.
    public async Task<List<LearningMaterial>> GetAllAsync()
    {
        var response = await _http.GetFromJsonAsync<List<LearningMaterial>>("api/LearningMaterial");
        return response ?? new List<LearningMaterial>();
    }
}