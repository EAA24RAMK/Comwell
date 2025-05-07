using Microsoft.AspNetCore.Mvc;
using Core.Models;
using ServerAPI.Repositories;

namespace ServerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _repo;

    public UserController(IUserRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        return Ok(await _repo.GetAllAsync());
    }

    [HttpGet("email/{email}")]
    public async Task<ActionResult<User>> GetByEmail(string email)
    {
        var user = await _repo.GetUserByEmailAsync(email);
        if (user == null) return NotFound();
        return Ok(user);
    }

    [HttpGet("role/{role}")]
    public async Task<ActionResult<List<User>>> GetByRole(string role)
    {
        var users = await _repo.GetByRoleAsync(role);
        return Ok(users);
    }

    [HttpGet("hotel/{hotel}")]
    public async Task<ActionResult<List<User>>> GetByHotel(string hotel)
    {
        var users = await _repo.GetByHotelAsync(hotel);
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<User>> Create(User user)
    {
        var created = await _repo.CreateAsync(user);
        return Ok(created);
    }
}