using Core.Models;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories;

namespace ServerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TemplateController : ControllerBase
{
    private readonly ITemplateRepository _templateRepo;
    
    public TemplateController(ITemplateRepository templateRepo)
    {
        _templateRepo = templateRepo;
    }

    [HttpGet("create")]
    public async Task<ActionResult> CreateStandardTemplate()
    {
        await _templateRepo.CreateStandardTemplateAsync();
        return Ok("Standardtemplate oprettet.");
    }

    [HttpGet]
    public async Task<ActionResult<List<Template>>> GetAllTemplatesAsync()
    {
        var templates = await _templateRepo.GetAllTemplatesAsync();
        return Ok(templates);
    } 
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Template>> GetTemplateById(int id)
    {
        var template = await _templateRepo.GetTemplateByIdAsync(id);
        if (template == null)
            return NotFound($"Ingen template med id = {id} blev fundet.");

        return Ok(template);
    }
}