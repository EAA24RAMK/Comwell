using Core.Models;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories;

namespace ServerAPI.Controllers;

// Controller overblik
// - Håndterer HTTP-requests relateret til læringsmaterialer.
// - Bruges til at uploade filer, tilføje links, hente filer, hente materialer og slette materialer.
// - Kommunikerer med LearningMaterialRepository.
[Route("api/[controller]")]
[ApiController]
public class LearningMaterialController : ControllerBase
{
    private readonly ILearningMaterialRepository _repo; // Instansvariabel til at kommunikere med repository

    // Konstruktør hvor repositories bliver injected.
    public LearningMaterialController(ILearningMaterialRepository repo)
    {
        _repo = repo;
    }

    // Returnerer: 200 OK med oprettet materiale eller 400 BadRequest hvis filen er tom.
    // Parametre:
    //   file – Den fil der skal uploades.
    //   title – Titel på materialet.
    //   subtaskId – ID på delmålet materialet tilknyttes.
    // Formål: Upload en fil til systemet og opret et læringsmateriale baseret på filen.
    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] string title, [FromForm] int subtaskId)
    {
        if (file == null || file.Length == 0)
            return BadRequest("Filen er tom"); // Returner fejl hvis ingen fil er valgt

        // Kopiér filen til et MemoryStream for at kunne sende den til GridFS
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        memoryStream.Position = 0; // Sæt stream tilbage til start

        // Upload fil til GridFS og få filens ID
        var fileId = await _repo.UploadFileAsync(memoryStream, file.FileName);

        // Opret et nyt læringsmateriale-objekt baseret på filen
        var material = new LearningMaterial
        {
            Title = title,
            SubtaskId = subtaskId,
            IsLink = false,
            FileName = file.FileName,
            FileId = fileId
        };

        var created = await _repo.CreateAsync(material); // Gem materialet i databasen
        return Ok(created); // Returner det oprettede materiale
    }

    // Returnerer: 200 OK med oprettet link eller 400 BadRequest hvis linket er ugyldigt.
    // Parametre: link – Et læringsmateriale-objekt der repræsenterer et link.
    // Formål: Tilføj et læringsmateriale der er et link, i stedet for en fil.
    [HttpPost("link")]
    public async Task<IActionResult> AddLink([FromBody] LearningMaterial link)
    {
        if (!link.IsLink || string.IsNullOrWhiteSpace(link.LinkUrl))
            return BadRequest("Linket er ugyldigt");

        var created = await _repo.CreateAsync(link);
        return Ok(created);
    }

    // Returnerer: Fildownload som stream.
    // Parametre: fileId – ID på filen der skal downloades.
    // Formål: Hent og download en fil fra GridFS ud fra filens ID.
    [HttpGet("download/{fileId}")]
    public async Task<IActionResult> Download(string fileId)
    {
        var stream = await _repo.GetFileStreamAsync(fileId); // Hent filen som stream
        return File(stream, "application/octet-stream", "downloaded_file"); // Returner fil som download
    }

    // Returnerer: 204 NoContent hvis succesfuld sletning, 404 NotFound hvis materialet ikke findes.
    // Parametre: id – ID på læringsmaterialet der skal slettes.
    // Formål: Slet et læringsmateriale og evt. tilhørende fil fra systemet.
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _repo.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
    
    // Returnerer: Liste med alle læringsmaterialer.
    // Formål: Hent alle læringsmaterialer (både filer og links) fra databasen.
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var materials = await _repo.GetAllAsync();
        return Ok(materials);
    }
}
