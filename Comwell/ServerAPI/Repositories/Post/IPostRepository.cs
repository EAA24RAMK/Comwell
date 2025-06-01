using Core.Models;

namespace ServerAPI.Repositories;

public interface IPostRepository
{
    Task<Post?> Create(Post post);
    void Delete(int id);
    List<Post> GetAll();
    List<Post> GetForUser(string username, string role); // bruges nu aktivt
}