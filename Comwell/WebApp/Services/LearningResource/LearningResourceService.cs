using Core.Models;
using System.Net.Http.Json;

namespace WebApp.Services;

public class LearningResourceService
{
    private readonly HttpClient _http;

    public LearningResourceService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<LearningResource>> GetAll()
    {
        return await _http.GetFromJsonAsync<List<LearningResource>>("api/learningresource");
    }

    public async Task<LearningResource> GetById(int id)
    {
        return await _http.GetFromJsonAsync<LearningResource>($"api/learningresource/{id}");
    }

    public async Task Create(LearningResource resource)
    {
        await _http.PostAsJsonAsync("api/learningresource", resource);
    }

    public async Task Delete(int id)
    {
        await _http.DeleteAsync($"api/learningresource/{id}");
    }
}