using Core.Models;

namespace WebApp.Services;

public interface IPostService
{
    Task<List<Post>> GetAllPostsAsync();
    Task CreatePostAsync(Post post);
}