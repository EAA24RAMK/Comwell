using Core.Models;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories;

namespace ServerAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LearningMaterialController : ControllerBase
{
    private readonly ILearningMaterialRepository _repo;

    public LearningMaterialController(ILearningMaterialRepository repo)
    {
        _repo = repo;
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string title, [FromForm] int subtaskId)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Filen er tom");

        // Kopiér IFormFile til MemoryStream
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        // Upload til GridFS
        var fileId = await _repo.UploadFileAsync(memoryStream, file.FileName);

        var material = new LearningMaterial
        {
            Title = title,
            SubtaskId = subtaskId,
            IsLink = false,
            FileName = file.FileName,
            FileId = fileId
        };

        var created = await _repo.CreateAsync(material);
        return Ok(created);
    }


    [HttpPost("link")]
    public async Task<IActionResult> AddLink([FromBody] LearningMaterial link)
    {
        if (!link.IsLink || string.IsNullOrWhiteSpace(link.LinkUrl))
            return BadRequest("Linket er ugyldigt");

        var created = await _repo.CreateAsync(link);
        return Ok(created);
    }

    [HttpGet("{subtaskId:int}")]
    public async Task<IActionResult> GetBySubtask(int subtaskId)
    {
        var materials = await _repo.GetBySubtaskIdAsync(subtaskId);
        return Ok(materials);
    }

    [HttpGet("download/{fileId}")]
    public async Task<IActionResult> Download(string fileId)
    {
        var stream = await _repo.GetFileStreamAsync(fileId);
        return File(stream, "application/octet-stream", "downloaded_file");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _repo.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var materials = await _repo.GetAllAsync();
        return Ok(materials);
    }
}
