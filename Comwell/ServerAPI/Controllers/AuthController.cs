using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories;
using Core.Models;

namespace ServerAPI.Controllers;
[ApiController]
[Route("api/[controller]")]

public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepo;

    // Dependency injection
    public AuthController(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }
    
    // IActionResult returner svar succes el. fejl 200, 400, 401 osv
    // Godkender bruger med email og password
    // Returner id, name osv. i loggedInUser
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userRepo.GetUserByEmailAsync(request.Email);
        if (user == null || user.Password != request.Password)
            return Unauthorized("Forkert loginoplysninger");

        return Ok(new {
            user.Id,
            user.Name,
            user.Email,
            user.Role,
            user.Hotel,
            user.Status,
            user.CreatedAt
        });
    }


    // Login dataet sendt fra frontend
    //   "email": "bruger@comwell.dk",
    // "password": "123"
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}