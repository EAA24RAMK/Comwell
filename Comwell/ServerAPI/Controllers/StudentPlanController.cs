using Core.Models;
using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories;

namespace ServerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentPlanController : ControllerBase
{
    private readonly IStudentPlanRepository _studentPlanRepo;
    
    public StudentPlanController(IStudentPlanRepository studentPlanRepo)
    {
        _studentPlanRepo = studentPlanRepo;
    }
    
    [HttpPost]
    public async Task<ActionResult<StudentPlan>> CreateStudentPlan(StudentPlan createPlan)
    {
        if (createPlan == null) return BadRequest("Plan data mangler");
            
        await _studentPlanRepo.CreateStudentPlanAsync(createPlan);
        return Ok("Planen er oprettet");
    }
    
    [HttpGet]
    public async Task<ActionResult<List<StudentPlan>>> GetAll()
    {
        return Ok(await _studentPlanRepo.GetAllPlansAsync());
    }
    
    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<List<StudentPlan>>> GetByStudent(int studentId)
    {
        return Ok(await _studentPlanRepo.GetPlansByStudentAsync(studentId));
    }

    [HttpGet("hotel/{hotel}")]
    public async Task<ActionResult<List<StudentPlan>>> GetByHotel(string hotel)
    {
        return Ok(await _studentPlanRepo.GetPlansByHotelAsync(hotel));
    }

}