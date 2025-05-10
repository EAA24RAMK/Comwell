using System.Net.Http.Json;
using Core.Models;

namespace WebApp.Services;

public class TemplateService : ITemplateService
{
    private readonly HttpClient _http;
    public TemplateService(HttpClient http)
    {
        _http = http;
    }
    
    public async Task<List<Template>> GetAllTemplatesAsync()
    {
        return await _http.GetFromJsonAsync<List<Template>>("api/template") ?? new();
    }

    public async Task<Template?> GetTemplateByIdAsync(int id)
    {
        return await _http.GetFromJsonAsync<Template?>($"api/template/{id}");
    }
}