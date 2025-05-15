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

    public async Task<List<Notification>> GetByUserIdAsync(int userId)
    {
        return await _notifications.Find(n => n.RelatedUserId == userId).ToListAsync();
    }

    public async Task<Notification?> GetByNotificationIdAsync(int id)
    {
        return await _notifications.Find(n => n.Id == id).FirstOrDefaultAsync();
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
}