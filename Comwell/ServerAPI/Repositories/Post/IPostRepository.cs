using Core.Models;

namespace ServerAPI.Repositories;

public interface IPostRepository
{
    void Create(Post post);
    List<Post> GetAll();
    List<Post> GetForUser(string username, string role);
}