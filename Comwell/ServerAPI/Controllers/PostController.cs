using Core.Models;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories;

namespace ServerAPI.Controllers;

// Controller overblik
// - Håndterer HTTP-requests relateret til opslag (Post).
// - Bruges af frontend til at oprette, hente og slette opslag.
// - Kalder PostRepository for at arbejde med MongoDB.
[ApiController]
[Route("api/[controller]")]
public class PostsController : ControllerBase
{
    private readonly IPostRepository _postRepository; // Instansvariabel til at kommunikere med opslag i databasen

    // Konstruktør hvor PostRepository bliver injected, så controlleren kan bruge den.
    public PostsController(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    // Returnerer: Det oprettede opslag eller fejl (BadRequest).
    // Parametre: post – Opslaget der skal oprettes (sendes fra frontend).
    // Formål: Opretter et nyt opslag i databasen. 
    // Svarer med det oprettede opslag hvis det lykkes.
    [HttpPost]
    public async Task<ActionResult<Post>> CreatePost([FromBody] Post post)
    {
        if (post == null)
            return BadRequest();

        var createdPost = await _postRepository.Create(post);
        return Ok(createdPost);
    }

    // Returnerer: Liste med alle opslag.
    // Formål: Henter alle opslag fra databasen.
    [HttpGet]
    public ActionResult<List<Post>> GetAllPosts()
    {
        var posts = _postRepository.GetAll();
        return Ok(posts);
    }
    
    // Parametre: id – ID på opslaget der skal slettes.
    // Formål: Sletter et opslag fra databasen baseret på dets ID.
    [HttpDelete("{id}")]
    public IActionResult DeletePost(int id)
    {
        _postRepository.Delete(id);
        return Ok();
    }
    
    // Returnerer: Liste af opslag målrettet brugeren.
    // Parametre:
    //   username – Brugerens e-mail.
    //   role – Brugerens rolle (fx "Elev", "HR", "Køkkenchef").
    // Formål: Henter opslag der er relevante for den specifikke bruger:
    // - HR og Køkkenchef får alle opslag.
    // - Andre får kun opslag målrettet dem selv eller opslag uden specifik målgruppe.
    [HttpGet("mine")]
    public ActionResult<List<Post>> GetMyPosts([FromQuery] string username, [FromQuery] string role)
    {
        var posts = _postRepository.GetForUser(username, role);
        return Ok(posts);
    }
}