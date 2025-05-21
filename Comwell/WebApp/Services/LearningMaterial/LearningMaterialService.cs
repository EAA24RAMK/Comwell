using System.Net.Http.Headers;
using System.Net.Http.Json;
using Core.Models;

namespace WebApp.Services;

public class LearningMaterialService : ILearningMaterialService
{
    private readonly HttpClient _http;

    public LearningMaterialService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<LearningMaterial>> GetBySubtaskIdAsync(int subtaskId)
    {
        var result = await _http.GetFromJsonAsync<List<LearningMaterial>>($"api/LearningMaterial/{subtaskId}");
        return result ?? new List<LearningMaterial>();
    }

    public async Task<LearningMaterial?> UploadFileAsync(Stream fileStream, string fileName, string title, int subtaskId)
    {
        var content = new MultipartFormDataContent();

        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        content.Add(fileContent, "file", fileName);
        content.Add(new StringContent(title), "title");
        content.Add(new StringContent(subtaskId.ToString()), "subtaskId");

        var response = await _http.PostAsync("api/LearningMaterial/upload", content);
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<LearningMaterial>();
    }

    public async Task<LearningMaterial?> AddLinkAsync(LearningMaterial link)
    {
        var response = await _http.PostAsJsonAsync("api/LearningMaterial/link", link);
        if (!response.IsSuccessStatusCode) return null;

        return await response.Content.ReadFromJsonAsync<LearningMaterial>();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/LearningMaterial/{id}");
        return response.IsSuccessStatusCode;
    }
    
    public async Task<List<LearningMaterial>> GetAllAsync()
    {
        var response = await _http.GetFromJsonAsync<List<LearningMaterial>>("api/LearningMaterial");
        return response ?? new List<LearningMaterial>();
    }
}