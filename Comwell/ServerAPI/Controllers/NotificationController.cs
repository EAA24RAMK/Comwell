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
        var notifications = await _notificationRepo.GetByUserIdAsync(userId);
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

                if (daysUntilDeadline <= 2 && daysUntilDeadline >= 0)
                {
                    var student = allUsers.FirstOrDefault(u => u.Id == plan.StudentId);
                    if (student == null) continue;

                    var message = $"Målet '{goal.Title}' skal være færdigt om 2 dage.";

                    // Elev får notifikation
                    await _notificationRepo.AddAsync(new Notification
                    {
                        Message = message,
                        Deadline = goal.Deadline,
                        RelatedUserId = student.Id,
                        PlanId = plan.Id,
                        GoalId = goal.Id,
                        CreatedAt = DateTime.Now
                    });

                    // HR-brugere får også notifikation
                    var hrUsers = allUsers.Where(u => u.Role == "HR");
                    foreach (var hr in hrUsers)
                    {
                        await _notificationRepo.AddAsync(new Notification
                        {
                            Message = $"(HR) {student.Name}: {message}",
                            Deadline = goal.Deadline,
                            RelatedUserId = hr.Id,
                            PlanId = plan.Id,
                            GoalId = goal.Id,
                            CreatedAt = DateTime.Now
                        });
                    }
                }
            }
        }

        return Ok("Notifikationer genereret.");
    }

}