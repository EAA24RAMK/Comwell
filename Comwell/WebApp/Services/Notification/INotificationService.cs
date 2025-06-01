using Core.Models;

namespace WebApp.Services;

public interface INotificationService
{
    Task<List<Notification>> GetNotificationsByUserAsync(int userId);
    Task DeleteNotificationForUserAsync(int notificationId, int userId);
    Task GenererateNotificationsAsync();
}