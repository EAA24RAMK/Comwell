using Microsoft.AspNetCore.Mvc;
using Core.Models;
using ServerAPI.Repositories;

namespace ServerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _repo;

    // Dependency injection
    public UserController(IUserRepository repo)
    {
        _repo = repo;
    }

    // Returner alle brugere fra databasen
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        return Ok(await _repo.GetAllAsync());
    }

    // Finder en bruger ud fra email i databasen
    [HttpGet("email/{email}")]
    public async Task<ActionResult<User>> GetByEmail(string email)
    {
        var user = await _repo.GetUserByEmailAsync(email);
        if (user == null) return NotFound();
        return Ok(user);
    }

    // Returner alle brugere med en bestemt rolle
    [HttpGet("role/{role}")]
    public async Task<ActionResult<List<User>>> GetByRole(string role)
    {
        var users = await _repo.GetByRoleAsync(role);
        return Ok(users);
    }

    // Returner alle brugere i en bestemt hotel
    [HttpGet("hotel/{hotel}")]
    public async Task<ActionResult<List<User>>> GetByHotel(string hotel)
    {
        var users = await _repo.GetByHotelAsync(hotel);
        return Ok(users);
    }

    // Opretter en ny bruger i databasen
    [HttpPost]
    public async Task<ActionResult<User>> Create(User user)
    {
        var created = await _repo.CreateAsync(user);
        return Ok(created);
    }
    
    // Sletter en bruger fra databasen baseret på userID
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repo.DeleteAsync(id);
        if (!deleted) return NotFound();
        return Ok();
    }
    
    [HttpGet("id/{id}")]
    public async Task<ActionResult<User>> GetById(int id)
    {
        var user = await _repo.GetUserByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }
    
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
    {
        var updated = await _repo.UpdateStatusAsync(id, newStatus);
        if (!updated) return NotFound();
        return NoContent();
    }
}