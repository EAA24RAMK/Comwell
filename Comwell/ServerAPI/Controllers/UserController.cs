using Microsoft.AspNetCore.Mvc; // Giver adgang til API-funktioner som ControllerBase, IActionResult osv
using Core.Models;
using ServerAPI.Repositories;

namespace ServerAPI.Controllers;

// Controller der håndterer API-endpoints relateret til brugere.
// Bruges af frontend til at hente, oprette, slette og opdatere brugere via HTTP-anmodninger.

[ApiController]                         // Fortæller .NET at dette er en API-controller
[Route("api/[controller]")]     // URL bliver /api/user, fordi controlleren hedder UserController
public class UserController : ControllerBase
{
    // Instansvariabel der repræsenterer vores bruger-repository.
    // Bruges til at kommunikere med databasen via IUserRepository.
    private readonly IUserRepository _repo;

    // Konstruktør hvor IUserRepository bliver "injectet" fra systemet (dependency injection).
    // Det gør det muligt at arbejde med brugerdata uden selv at oprette databasen her.
    public UserController(IUserRepository repo)
    {
        _repo = repo;
    }

    // Returnerer: Liste over alle brugere i databasen.
    // Formål: Bruges til at hente en samlet oversigt over alle brugere – fx i UserPage.
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        return Ok(await _repo.GetAllAsync());
    }

    // Returnerer: Den nyoprettede bruger inkl. ID og oprettelsesdato.
    // Parametre: user – det brugerobjekt som skal oprettes.
    // Formål: Bruges til at oprette en ny bruger fra frontend, fx i tilføj bruger modal.
    [HttpPost]
    public async Task<ActionResult<User>> Create(User user)
    {
        var created = await _repo.CreateAsync(user);
        return Ok(created);
    }
    
    // Returnerer: OK hvis brugeren blev slettet, ellers NotFound hvis brugeren ikke findes.
    // Parametre: id – ID’et på den bruger der skal slettes.
    // Formål: Bruges når en medarbejder skal fjernes fra systemet.
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repo.DeleteAsync(id);
        if (!deleted) return NotFound();
        return Ok();
    }
    
    // Returnerer: En bruger baseret på ID eller NotFound hvis ingen findes.
    // Parametre: id – ID på den ønskede bruger.
    // Formål: Bruges når man vil se eller redigere en specifik bruger.
    [HttpGet("id/{id}")]
    public async Task<ActionResult<User>> GetById(int id)
    {
        var user = await _repo.GetUserByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(user);
    }
    
    // Returnerer: NoContent hvis opdateringen lykkes, ellers NotFound.
    // Parametre:
    //   id – ID på brugeren der skal opdateres.
    //   newStatus – Den nye status-værdi (fx "Aktiv", "Inaktiv").
    // Formål: Bruges når man fx midlertidigt skal deaktivere eller genaktivere en bruger.
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus) // [FromBody] bruges til at hente data fra body i HTTP-anmodningen.
    {
        var updated = await _repo.UpdateStatusAsync(id, newStatus);
        if (!updated) return NotFound();
        return NoContent();
    }
}