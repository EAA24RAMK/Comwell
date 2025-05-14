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
    public IActionResult CreatePost(Post post)
    {
        if (post == null) return BadRequest();

        _postRepository.Create(post);
        return Ok();
    }

    // GET: api/posts Load posts
    [HttpGet]
    public ActionResult<List<Post>> GetAllPosts()
    {
        var posts = _postRepository.GetAll();
        return Ok(posts);
    }
}