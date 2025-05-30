using Core.Models;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories;

namespace ServerAPI.Controllers;

// Controller der håndterer API-endpoints relateret til template (skabelon).
// Bruges af frontend til at hente og vise templates via HTTP-anmodninger.
[ApiController]
[Route("api/[controller]")]
public class TemplateController : ControllerBase
{
    // Instansvariabel der bruges til at kommunikere med ITemplateRepository
    private readonly ITemplateRepository _templateRepo;
    
    // Dependency injection:
    // Konstruktøren modtager ITemplateRepository via dependency injection.
    // Det gør det muligt at tilgå repository-metoder uden at oprette repository-objektet direkte her.
    public TemplateController(ITemplateRepository templateRepo)
    {
        _templateRepo = templateRepo;
    }

    // Returnerer: En liste med alle templates i databasen.
    // Formål: Bruges til at hente alle templates, fx i opret-plan dropdownen.
    [HttpGet]
    public async Task<ActionResult<List<Template>>> GetAllTemplatesAsync()
    {
        var templates = await _templateRepo.GetAllTemplatesAsync();
        return Ok(templates);
    } 
    
    // Returnerer: En enkelt template baseret på ID, eller NotFound hvis den ikke findes.
    // Parametre: id – template-ID'et vi søger efter.
    // Formål: Bruges til at hente en specifik template fra databasen.
    [HttpGet("{id}")]
    public async Task<ActionResult<Template>> GetTemplateById(int id)
    {
        var template = await _templateRepo.GetTemplateByIdAsync(id);
        if (template == null)
            return NotFound($"Ingen template med id = {id} blev fundet.");

        return Ok(template);
    }
}