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

                    // --- Tjek om denne notifikation allerede findes for eleven ---
                    var existingStudentNotes = await _notificationRepo.GetByUserIdAsync(student.Id);
                    bool alreadyExistsForStudent = existingStudentNotes.Any(n =>
                        n.GoalId == goal.Id &&
                        n.PlanId == plan.Id &&
                        n.Deadline.Date == goal.Deadline.Date &&
                        n.Message == message
                    );

                    if (!alreadyExistsForStudent)
                    {
                        await _notificationRepo.AddAsync(new Notification
                        {
                            Message = message,
                            Deadline = goal.Deadline,
                            RelatedUserId = student.Id,
                            PlanId = plan.Id,
                            GoalId = goal.Id,
                            CreatedAt = DateTime.Now
                        });
                    }

                    // --- HR-brugere ---
                    var hrUsers = allUsers.Where(u => u.Role == "HR");
                    foreach (var hr in hrUsers)
                    {
                        var hrMessage = $"(HR) {student.Name}: {message}";
                        var existingHrNotes = await _notificationRepo.GetByUserIdAsync(hr.Id);
                        bool alreadyExistsForHr = existingHrNotes.Any(n =>
                            n.GoalId == goal.Id &&
                            n.PlanId == plan.Id &&
                            n.Deadline.Date == goal.Deadline.Date &&
                            n.Message == hrMessage
                        );

                        if (!alreadyExistsForHr)
                        {
                            await _notificationRepo.AddAsync(new Notification
                            {
                                Message = hrMessage,
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
        }

        return Ok("Notifikationer genereret.");
    }
    
}