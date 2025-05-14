using Core.Models;
using System.Net.Http.Json;

namespace WebApp.Services;

public class PostService : IPostService
{
    private readonly HttpClient _http;

    public PostService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Post>> GetAllPostsAsync()
    {
        return await _http.GetFromJsonAsync<List<Post>>("api/posts") ?? new();
    }

    public async Task CreatePostAsync(Post post)
    {
        await _http.PostAsJsonAsync("api/posts", post);
    }
}