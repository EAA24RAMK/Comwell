using Core.Models;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories;

namespace ServerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentPlanController : ControllerBase
{
    private readonly IStudentPlanRepository _studentPlanRepo;
    
    // Dependency injection
    public StudentPlanController(IStudentPlanRepository studentPlanRepo)
    {
        _studentPlanRepo = studentPlanRepo;
    }
    
    [HttpPost] // Endpoint: POST api/studentplan
    public async Task<ActionResult<StudentPlan>> CreateStudentPlan(StudentPlan createPlan)
    {
        if (createPlan == null) return BadRequest("Plan data mangler"); // Hvis plan data mangler returnerer vi 400
            
        await _studentPlanRepo.CreateStudentPlanAsync(createPlan); // Gemmer planen i databasen
        return Ok("Planen er oprettet");
    }
    
    // Returner alle elevplaner fra databasen
    [HttpGet]
    public async Task<ActionResult<List<StudentPlan>>> GetAll()
    {
        return Ok(await _studentPlanRepo.GetAllPlansAsync());
    }
    
    // Henter elevplaner ud fra studentID (for en elev)
    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<List<StudentPlan>>> GetByStudent(int studentId)
    {
        return Ok(await _studentPlanRepo.GetPlansByStudentAsync(studentId));
    }

    // Henter elevplaner ud fra hotel
    [HttpGet("hotel/{hotel}")]
    public async Task<ActionResult<List<StudentPlan>>> GetByHotel(string hotel)
    {
        return Ok(await _studentPlanRepo.GetPlansByHotelAsync(hotel));
    }
    
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
    
    // sletter en hel studentplan
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _studentPlanRepo.DeleteStudentPlanAsync(id);
        if (!deleted)
            return NotFound("Plan ikke fundet");

        return Ok("Plan slettet");
    }
    
}