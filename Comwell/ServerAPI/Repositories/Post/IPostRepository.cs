using Core.Models;

namespace ServerAPI.Repositories;

public interface IPostRepository
{
    void Create(Post post);
    
    void Delete(int id);

    List<Post> GetAll();
    
    List<Post> GetForUser(string username, string role);
    
}