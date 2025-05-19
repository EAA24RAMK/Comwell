using Core.Models;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories;

namespace ServerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository _postRepository;

    public PostsController(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    // POST: api/posts
    [HttpPost]
    public IActionResult CreatePost([FromBody] Post post)
    {
        if (post == null)
            return BadRequest();

        _postRepository.Create(post);
        return Ok();
    }

    // GET: api/posts
    [HttpGet]
    public ActionResult<List<Post>> GetAllPosts()
    {
        var posts = _postRepository.GetAll();
        return Ok(posts);
    }

    // DELETE: api/posts/{id}
    [HttpDelete("{id}")]
    public IActionResult DeletePost(int id)
    {
        _postRepository.Delete(id);
        return Ok();
    }
    
    [HttpGet("mine")]
    public ActionResult<List<Post>> GetMyPosts([FromQuery] string username, [FromQuery] string role)
    {
        var posts = _postRepository.GetForUser(username, role);
        return Ok(posts);
    }
}