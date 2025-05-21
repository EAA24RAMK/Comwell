using Core.Models;
using MongoDB.Driver;

namespace ServerAPI.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly IMongoCollection<Notification> _notifications;
    
    public NotificationRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _notifications = db.GetCollection<Notification>("notification");
    }
    
    public async Task<List<Notification>> GetAllAsync()
    {
        return await _notifications.Find(_ => true).ToListAsync();
    }

    public async Task<List<Notification>> GetForUserAsync(int userId)
    {
        var today = DateTime.Today;
        return await _notifications.Find(n => n.NotifyUserId.Contains(userId) && n.Deadline >= today).ToListAsync();
    }

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

    public async Task UpdateAsync(Notification notification)
    {
        await _notifications.ReplaceOneAsync(n => n.Id == notification.Id, notification);
    }

    public async Task DeleteAsync(int id)
    {
        await _notifications.DeleteOneAsync(n => n.Id == id);
    }

    public async Task RemoveGoalNotificationForAllUsersAsync(int planId, int goalId)
    {
        var matching = await _notifications.Find(n => n.PlanId == planId && n.GoalId == goalId).ToListAsync();

        foreach (var note in matching)
        {
            note.NotifyUserId.Clear(); // ingen skal længere se den
            await UpdateAsync(note);   // behold notifikationen
        }
    }

    public async Task MarkAsDeletedForUserAsync(int notificationId, int userId)
    {
        var notification = await _notifications.Find(n => n.Id == notificationId).FirstOrDefaultAsync();
        if (notification == null) return;

        if (!notification.DeletedByUserIds.Contains(userId))
            notification.DeletedByUserIds.Add(userId);

        notification.NotifyUserId.Remove(userId); // vis den ikke mere

        if (!notification.NotifyUserId.Any())
        {
            await _notifications.DeleteOneAsync(n => n.Id == notificationId);
        }
        else
        {
            await UpdateAsync(notification);
        }
    }

    public async Task CreateNotificationAsync(StudentPlan plan, Goal goal, List<User> allUsers)
    {
        var studentUser = allUsers.FirstOrDefault(u => u.Id == plan.StudentId);
        var studentName = studentUser?.Name ?? "Ukendt elev";

        var message = $"{studentName}: Målet \"{goal.Title}\" nærmer sig.";

        // Find eksisterende notifikation
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
            // 🛑 Hvis NotifyUserId allerede er tom → gør ingenting
            if (!existing.NotifyUserId.Any())
                return;

            // Fjern dem der tidligere har slettet den
            var visibleTo = notifyUsers.Except(existing.DeletedByUserIds).ToList();

            // Hvis ingen skal se den → gør ingenting
            if (!visibleTo.Any())
                return;

            // Opdatér eksisterende notifikation
            existing.NotifyUserId = visibleTo;
            existing.Message = message;
            await UpdateAsync(existing);
            return;
        }

        // 🆕 Opret ny notifikation
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