using Core.Models;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories;

namespace ServerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LearningResourceController : ControllerBase
{
    private readonly ILearningResourceRepository _repository;

    public LearningResourceController(ILearningResourceRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public ActionResult<List<LearningResource>> GetAll()
    {
        return _repository.GetAll();
    }

    [HttpGet("{id}")]
    public ActionResult<LearningResource> GetById(int id)
    {
        var resource = _repository.GetById(id);
        if (resource == null)
        {
            return NotFound();
        }

        return resource;
    }

    [HttpPost]
    public IActionResult Create(LearningResource resource)
    {
        _repository.Create(resource);
        return CreatedAtAction(nameof(GetById), new { id = resource.Id }, resource);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var existing = _repository.GetById(id);
        if (existing == null)
        {
            return NotFound();
        }

        _repository.Delete(id);
        return NoContent();
    }
}