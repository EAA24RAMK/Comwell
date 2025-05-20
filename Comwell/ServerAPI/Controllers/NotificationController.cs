using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories;
using Core.Models;

namespace ServerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationRepository _notificationRepo;
    private readonly IStudentPlanRepository _studentPlanRepo;
    private readonly IUserRepository _userRepo;

    // Dependency injection
    public NotificationController(INotificationRepository notificationRepo, IStudentPlanRepository studentPlanRepo, IUserRepository userRepo)
    {
        _notificationRepo = notificationRepo;
        _studentPlanRepo = studentPlanRepo;
        _userRepo = userRepo;
    }

    // Returner alle notifikationer fra databasen
    [HttpGet]
    public async Task<ActionResult<List<Notification>>> GetAllNotifications()
    {
        var notifications = await _notificationRepo.GetAllAsync();
        return Ok(notifications);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<Notification>>> GetNotificationsByUser(int userId)
    {
        var notifications = await _notificationRepo.GetForUserAsync(userId);
        return Ok(notifications);
    }

    [HttpPost]
    public async Task<IActionResult> Add(Notification notification)
    {
        await _notificationRepo.AddAsync(notification);
        return Ok();
    }
    
    [HttpPut]
    public async Task<IActionResult> Update(Notification notification)
    {
        await _notificationRepo.UpdateAsync(notification);
        return Ok();
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _notificationRepo.DeleteAsync(id);
        return Ok();
    }
    
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateNotifications()
    {
        var allPlans = await _studentPlanRepo.GetAllPlansAsync();
        var allUsers = await _userRepo.GetAllAsync();
        var today = DateTime.Today;

        foreach (var plan in allPlans)
        {
            foreach (var goal in plan.Goals)
            {
                var daysUntilDeadline = (goal.Deadline.Date - today).TotalDays;

                //Generer notifikationer 5 dage før og 1 dag før.
                if (daysUntilDeadline == 5 || daysUntilDeadline == 1)
                {
                    await _notificationRepo.CreateNotificationAsync(plan, goal, allUsers);
                }
            }
        }

        return Ok("Notifikationer genereret.");
    }
    [HttpPost("delete-for-user")]
    public async Task<IActionResult> DeleteForUser([FromBody] DeleteNotificationRequest request)
    {
        await _notificationRepo.MarkAsDeletedForUserAsync(request.NotificationId, request.UserId);
        return Ok();
    }
    
    public class DeleteNotificationRequest
    {
        public int NotificationId { get; set; }
        public int UserId { get; set; }
    }
}