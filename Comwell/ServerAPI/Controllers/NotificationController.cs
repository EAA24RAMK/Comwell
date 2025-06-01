using Microsoft.AspNetCore.Mvc;
using ServerAPI.Repositories;
using Core.Models;

namespace ServerAPI.Controllers;

// Controller overblik
// - Håndterer HTTP-requests relateret til notifikationer.
// - Bruges af frontend til at hente, oprette, opdatere og generere notifikationer.
// - Kalder NotificationRepository for at arbejde med databasen.
[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    // Instansvariabler til at kommunikere med repositories
    private readonly INotificationRepository _notificationRepo;
    private readonly IStudentPlanRepository _studentPlanRepo;
    private readonly IUserRepository _userRepo;

    // Konstruktør hvor repositories bliver injected.
    public NotificationController(INotificationRepository notificationRepo, IStudentPlanRepository studentPlanRepo, IUserRepository userRepo)
    {
        _notificationRepo = notificationRepo;
        _studentPlanRepo = studentPlanRepo;
        _userRepo = userRepo;
    }
    
    // Returnerer: Liste af alle notifikationer.
    // Formål: Henter alle notifikationer fra databasen.
    [HttpGet]
    public async Task<ActionResult<List<Notification>>> GetAllNotifications()
    {
        var notifications = await _notificationRepo.GetAllAsync();
        return Ok(notifications);
    }

    // Returnerer: Liste af notifikationer målrettet en bestemt bruger.
    // Parametre: userId – Brugerens ID.
    // Formål: Henter aktive notifikationer for en specifik bruger.
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<Notification>>> GetNotificationsByUser(int userId)
    {
        var notifications = await _notificationRepo.GetForUserAsync(userId);
        return Ok(notifications);
    }
    
    // Parametre: notification – Den notifikation der skal oprettes.
    // Formål: Opretter en ny notifikation i databasen.
    [HttpPost]
    public async Task<IActionResult> Add(Notification notification)
    {
        await _notificationRepo.AddAsync(notification);
        return Ok();
    }
    
    // Parametre: notification – Den notifikation der skal opdateres.
    // Formål: Opdaterer en eksisterende notifikation i databasen.
    [HttpPut]
    public async Task<IActionResult> Update(Notification notification)
    {
        await _notificationRepo.UpdateAsync(notification);
        return Ok();
    }
    
    // Formål: Genererer notifikationer for alle elevplaner baseret på deadlines.
    // Notifikationer genereres 5 dage før og 1 dag før målets deadline.
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateNotifications()
    {
        var allPlans = await _studentPlanRepo.GetAllPlansAsync(); // Hent alle elevplaner
        var allUsers = await _userRepo.GetAllAsync();                  // Hent alle brugere
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
    
    // Parametre: request – Indeholder notificationId og userId.
    // Formål: Marker en notifikation som slettet for en bestemt bruger.
    [HttpPost("{notificationId}/delete-for-user/{userId}")]
    public async Task<IActionResult> DeleteForUser(int notificationId, int userId)
    {
        await _notificationRepo.MarkAsDeletedForUserAsync(notificationId, userId);
        return Ok();
    }
}