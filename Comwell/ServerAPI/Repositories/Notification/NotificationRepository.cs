using Core.Models;
using MongoDB.Driver;

namespace ServerAPI.Repositories;

// Repository overblik
// - Håndterer alt arbejde med notifikationer i MongoDB.
// - Kan hente, oprette, opdatere og slette notifikationer.
// - Bruges fx til påmindelser om deadlines for mål i elevplaner.
public class NotificationRepository : INotificationRepository
{
    // Instansvariabel der referer til Notification-collection i MongoDB
    private readonly IMongoCollection<Notification> _notifications;

    // Konstruktøren sørger for at etablere forbindelsen til MongoDB.
    // connection string og databasenavn læses fra appsettings.json.
    public NotificationRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _notifications = db.GetCollection<Notification>("notification");
    }
    
    // Formål: Henter alle notifikationer fra databasen.
    public async Task<List<Notification>> GetAllAsync()
    {
        return await _notifications.Find(_ => true).ToListAsync();
    }

    // Returnerer: Liste med notifikationer målrettet en bestemt bruger.
    // Parametre: userId – Brugerens ID.
    // Formål: Henter aktive notifikationer for brugeren, som ikke er forældede.
    public async Task<List<Notification>> GetForUserAsync(int userId)
    {
        var today = DateTime.Today;
        return await _notifications.Find(n => n.NotifyUserId.Contains(userId) && n.Deadline >= today).ToListAsync();
    }

    // Parametre: notification – Notifikationen der skal oprettes.
    // Formål: Opretter en ny notifikation med automatisk ID og indsætter den i databasen.
    public async Task AddAsync(Notification notification)
    {
        int maxId = 0;
        var allNotifications = await _notifications.Find(_ => true).ToListAsync();
        if (allNotifications.Any())
        {
            maxId = allNotifications.Max(n => n.Id);
        }
        
        notification.Id = maxId + 1;
        
        await _notifications.InsertOneAsync(notification);
    }

    // Parametre: notification – Notifikationen der skal opdateres.
    // Formål: Opdaterer en eksisterende notifikation i databasen baseret på ID.
    public async Task UpdateAsync(Notification notification)
    {
        await _notifications.ReplaceOneAsync(n => n.Id == notification.Id, notification);
    }

    // Parametre: planId – ID på elevplanen, goalId – ID på målet.
    // Formål: Fjerner en notifikation for alle brugere ved at tømme listen af brugere.
    // Bruges fx når et mål er afsluttet og notifikationen ikke længere er relevant.
    public async Task RemoveGoalNotificationForAllUsersAsync(int planId, int goalId)
    {
        var matching = await _notifications.Find(n => n.PlanId == planId && n.GoalId == goalId).ToListAsync();

        foreach (var note in matching)
        {
            note.NotifyUserId.Clear(); // ingen skal længere se den
            await UpdateAsync(note);   // behold notifikationen
        }
    }

    // Parametre: notificationId – ID på notifikationen, userId – ID på brugeren.
    // Formål: Markerer en notifikation som slettet for en bestemt bruger.
    // Hvis ingen brugere er tilbage → slet notifikationen helt.
    public async Task MarkAsDeletedForUserAsync(int notificationId, int userId)
    {
        var notification = await _notifications.Find(n => n.Id == notificationId).FirstOrDefaultAsync();
        if (notification == null) return;

        if (!notification.DeletedByUserIds.Contains(userId))
            notification.DeletedByUserIds.Add(userId); // Registrer at brugeren har slettet notifikationen

        notification.NotifyUserId.Remove(userId); // Fjern brugeren fra dem der kan se notifikationen

        if (!notification.NotifyUserId.Any())
        {
            await _notifications.DeleteOneAsync(n => n.Id == notificationId); // Slet notifikation hvis ingen brugere er tilbage
        }
        else
        {
            await UpdateAsync(notification);
        }
    }

    // Parametre:
    //   plan – Den elevplan målet hører til.
    //   goal – Det mål som notifikationen skal omhandle.
    //   allUsers – Liste over alle brugere (bruges til at finde modtagere).
    // Formål: Opretter eller opdaterer en notifikation om at et mål nærmer sig deadline.
    public async Task CreateNotificationAsync(StudentPlan plan, Goal goal, List<User> allUsers)
    {
        // Find navn på elev
        var studentUser = allUsers.FirstOrDefault(u => u.Id == plan.StudentId);
        var studentName = studentUser?.Name;

        var message = $"{studentName}: Målet \"{goal.Title}\" nærmer sig.";

        // Find eksisterende notifikation for dette mål
        var existing = await _notifications.Find(n =>
            n.PlanId == plan.Id &&
            n.GoalId == goal.Id &&
            n.Deadline == goal.Deadline
        ).FirstOrDefaultAsync();

        // Identificér brugere der skal have notifikationen
        var notifyUsers = new List<int>();

        // Elev
        notifyUsers.Add(plan.StudentId);

        // Køkkenchefer på samme hotel
        if (studentUser != null)
        {
            var chefs = allUsers
                .Where(u => u.Role == "Køkkenchef" && u.Hotel == studentUser.Hotel)
                .Select(u => u.Id);
            notifyUsers.AddRange(chefs);
        }

        // HR
        var hrUsers = allUsers.Where(u => u.Role == "HR").Select(u => u.Id);
        notifyUsers.AddRange(hrUsers);

        notifyUsers = notifyUsers.Distinct().ToList();

        // Hvis notifikation allerede findes
        if (existing != null)
        {
            // Hvis NotifyUserId allerede er tom, gør ingenting
            if (!existing.NotifyUserId.Any())
                return;

            // Fjern brugere der tidligere har slettet notifikationen
            var visibleTo = notifyUsers.Except(existing.DeletedByUserIds).ToList();

            // Hvis ingen tilbage, gør ingenting
            if (!visibleTo.Any())
                return;

            // Opdater eksisterende notifikation med ny besked og modtagere
            existing.NotifyUserId = visibleTo;
            existing.Message = message;
            await UpdateAsync(existing);
            return;
        }

        // Opret ny notifikation
        var notification = new Notification
        {
            Message = message,
            CreatedAt = DateTime.Now,
            Deadline = goal.Deadline,
            PlanId = plan.Id,
            GoalId = goal.Id,
            NotifyUserId = notifyUsers,
            DeletedByUserIds = new(),
            IsRead = false
        };

        await AddAsync(notification);
    }
}