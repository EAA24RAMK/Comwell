using Core.Models;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories;

namespace ServerAPI.Controllers;

/* Controller overblik
- Ansvarlig for at håndtere HTTP-requests, som handler om elevplaner (StudentPlan).
- Kalder metoder i StudentPlanRepository for at oprette, hente, opdatere og slette planer.
- Bruges af frontend til at arbejde med elevplaner, fx når HR opretter planer eller elever ser deres egne planer. */
[ApiController]
[Route("api/[controller]")]
public class StudentPlanController : ControllerBase
{
    // Instansvariabel til at kommunikere med StudentPlanRepository
    private readonly IStudentPlanRepository _studentPlanRepo;
    
    // Konstruktør hvor IStudentplanRepository bliver "injectet" fra systemet (dependency injection).
    // Det gør det muligt at arbejde med brugerdata uden selv at oprette databasen her.
    public StudentPlanController(IStudentPlanRepository studentPlanRepo)
    {
        _studentPlanRepo = studentPlanRepo;
    }
    // Returnerer: En oprettet elevplan ellers fejlbesked.
    // Parametre: createPlan – objekt med data til den plan der skal oprettes.
    // Formål: Bruges til at oprette en ny elevplan i databasen via en POST-request.
    [HttpPost] // Endpoint: POST api/studentplan
    public async Task<ActionResult<StudentPlan>> CreateStudentPlan(StudentPlan createPlan)
    {
        if (createPlan == null) return BadRequest("Plan data mangler");
            
        await _studentPlanRepo.CreateStudentPlanAsync(createPlan); // Gemmer planen i databasen
        return Ok("Planen er oprettet");
    }
    
    // Returnerer: En liste med alle elevplaner.
    // Formål: Bruges til at hente alle elevplaner, fx til elevplan-siden.
    [HttpGet]
    public async Task<ActionResult<List<StudentPlan>>> GetAll()
    {
        return Ok(await _studentPlanRepo.GetAllPlansAsync());
    }
    
    // Returnerer: En liste med elevplaner for en specifik elev.
    // Parametre: studentId – ID på eleven.
    // Formål: Bruges til at hente alle planer, der tilhører en bestemt elev.
    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<List<StudentPlan>>> GetByStudent(int studentId)
    {
        return Ok(await _studentPlanRepo.GetPlansByStudentAsync(studentId));
    }

    // Returnerer: En liste med elevplaner for et specifikt hotel.
    // Parametre: hotel – Navn på hotellet.
    // Formål: Bruges til at hente alle planer for elever tilknyttet et bestemt hotel.
    [HttpGet("hotel/{hotel}")]
    public async Task<ActionResult<List<StudentPlan>>> GetByHotel(string hotel)
    {
        return Ok(await _studentPlanRepo.GetPlansByHotelAsync(hotel));
    }
    
    // Returnerer: En opdateret elevplan, ellers fejlbesked.
    // Parametre: 
    //   id – ID på planen der skal opdateres.
    //   updatedPlan – Objekt med de nye data til planen.
    // Formål: Bruges til at opdatere en eksisterende elevplan.
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] StudentPlan updatedPlan)
    {
        var existing = await _studentPlanRepo.GetPlanByIdAsync(id);
        if (existing == null)
            return NotFound("Planen blev ikke fundet");

        updatedPlan.Id = id; // bevar ID
        await _studentPlanRepo.UpdateStudentPlanAsync(updatedPlan);

        return NoContent();
    }
    
    // Returnerer: Sletter en elevplan, ellers fejlbesked.
    // Parametre: id – ID på planen der skal slettes.
    // Formål: Bruges til at slette en elevplan fra databasen.
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _studentPlanRepo.DeleteStudentPlanAsync(id);
        if (!deleted)
            return NotFound("Plan ikke fundet");

        return Ok("Plan slettet");
    }
    
    // Returnerer: Den opdaterede plan, som er godkendt af køkkenchef.
    // Parametre: id – ID på planen der skal godkendes af køkkenchef.
    // Formål: Bruges til at markere en elevplan som godkendt af køkkenchef.
    [HttpPut("{id}/approve-by-chef")]
    public async Task<IActionResult> ApproveByChef(int id)
    {
        var plan = await _studentPlanRepo.GetPlanByIdAsync(id);
        if (plan == null) return NotFound();

        plan.IsApprovedByChef = true;
        await _studentPlanRepo.UpdateStudentPlanAsync(plan);

        return Ok(plan);
    }
}