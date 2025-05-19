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

    public async Task DeleteAllForGoalAsync(int planId, int goalId)
    {
        await _notifications.DeleteManyAsync(n =>
            n.PlanId == planId && n.GoalId == goalId);
    }

    public async Task CreateNotificationAsync(StudentPlan plan, Goal goal, List<User> allUsers)
    {
        var studentUser = allUsers.FirstOrDefault(u => u.Id == plan.StudentId);
        var studentName = studentUser?.Name ?? "Ukendt elev";

        var message = $"{studentName}: Målet \"{goal.Title}\" nærmer sig.";


        // Tjek om en notifikation med samme PlanId, GoalId og Message allerede findes
        var existing = await _notifications.Find(n =>
            n.PlanId == plan.Id &&
            n.GoalId == goal.Id &&
            n.Deadline == goal.Deadline &&
            n.Message == message
        ).FirstOrDefaultAsync();

        if (existing != null)
            return;

        var notifyUsers = new List<int>();

        // Elev
        notifyUsers.Add(plan.StudentId);

        // Køkkenchefer på samme hotel
        var student = allUsers.FirstOrDefault(u => u.Id == plan.StudentId);
        if (student != null)
        {
            var chefs = allUsers
                .Where(u => u.Role == "Køkkenchef" && u.Hotel == student.Hotel)
                .Select(u => u.Id);
            notifyUsers.AddRange(chefs);
        }

        // HR
        var hrUsers = allUsers.Where(u => u.Role == "HR").Select(u => u.Id);
        notifyUsers.AddRange(hrUsers);

        var notification = new Notification
        {
            Message = message,
            CreatedAt = DateTime.Now,
            Deadline = goal.Deadline,
            PlanId = plan.Id,
            GoalId = goal.Id,
            NotifyUserId = notifyUsers.Distinct().ToList(),
            IsRead = false
        };

        await AddAsync(notification);
    }
}