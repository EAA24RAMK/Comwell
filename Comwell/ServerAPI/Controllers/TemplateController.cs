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
    
    [HttpPost]
    public async Task<ActionResult<Template>> CreateTemplate(Template createTemplate)
    {
        if (createTemplate == null || string.IsNullOrWhiteSpace(createTemplate.Title))
            return BadRequest("Ugyldig skabelon");
        
        await _templateRepo.CreateAsync(createTemplate);
        return Ok("Skabelon oprettet");
    }

    
    [HttpGet]
    public async Task<ActionResult<List<Template>>> GetAllTemplates()
    {
        return Ok(await _templateRepo.GetAllAsync());
    }
    
    [HttpGet("{id}")]
    public async Task<ActionResult<Template>> GetTemplateById(string id)
    {
        var template = await _templateRepo.GetByIdAsync(id);
        if (template == null) return NotFound("Skabelon ikke fundet");
        return Ok(template);
    }
}