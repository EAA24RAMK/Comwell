using System.Net.Http.Json;
using Core.Models;

namespace WebApp.Services;

// Denne service bruges i frontend til at hente templates fra backenden.
// Sender HTTP-requests til TemplateController i backend.
// Bruges fx i CreateStudentPlanPage.razor.
public class TemplateService : ITemplateService
{
    // Instansvariabel til at sende HTTP-anmodninger til backendens API
    private readonly HttpClient _http;
    
    // Konstruktøren hvor HttpClient bliver injected.
    public TemplateService(HttpClient http)
    {
        _http = http;
    }
    
    // Returnerer: En liste med alle templates fra backendens API.
    // Formål: Bruges til at hente alle templates, fx til opret plan dropdown.
    public async Task<List<Template>> GetAllTemplatesAsync()
    {
        return await _http.GetFromJsonAsync<List<Template>>("api/template") ?? new();
    }

    // Returnerer: En enkelt template baseret på ID, eller null hvis den ikke findes.
    // Parametre: id – ID på den template der skal hentes.
    // Formål: Bruges til at hente en specifik template.
    public async Task<Template?> GetTemplateByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<Template?>($"api/template/{id}");
    }
}