using Microsoft.AspNetCore.Mvc; // Giver adgang til ControllerBase og API-funktionalitet
using ServerAPI.Repositories;
using Core.Models;

namespace ServerAPI.Controllers;
// Controlleren håndterer autentificering (login).
// Den bruges af frontend til at logge brugere ind via en POST-request til /api/auth/login.

[ApiController]                         // Fortæller .NET at dette er en API-controller
[Route("api/[controller]")]     // URL bliver /api/auth, fordi controlleren hedder AuthController
public class AuthController : ControllerBase 
{
    // Instansvariabel der bruges til at kommunikere med bruger-databasen via IUserRepository.
    private readonly IUserRepository _userRepo;

    // Konstruktør hvor IUserRepository bliver "injectet" fra systemet (dependency injection).
    // Det gør det muligt at arbejde med brugerdata uden selv at oprette databasen her.
    public AuthController(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }
    
    // Returnerer: IActionResult – enten OK (200) med brugerdata eller Unauthorized (401) hvis login fejler.
    // Parametre: request – indeholder e-mail og password sendt fra frontend.
    // Formål: Tjekker om en bruger eksisterer med de indtastede loginoplysninger og returnerer brugerdata.
    // Bruges: Når en bruger forsøger at logge ind på systemet via LoginPage.
    [HttpPost("login")]
    public async Task<IActionResult> Login(Login request)
    {
        // Finder brugeren med den angivne e-mail
        var user = await _userRepo.GetUserByEmailAsync(request.Email);
        if (user == null || user.Password != request.Password)
            return Unauthorized("Forkert loginoplysninger");

        // Returnerer et objekt med de nødvendige brugeroplysninger
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
}