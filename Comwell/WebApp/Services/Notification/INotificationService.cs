using Core.Models;

namespace WebApp.Services;

public interface INotificationService
{
    Task<List<Notification>> GetAllNotificationsAsync();
    Task<List<Notification>> GetNotificationsByUserAsync(int userId);
    Task AddNotificationAsync(Notification notification);
    Task UpdateNotificationAsync(Notification notification);
    Task DeleteNotificationAsync(int id);
}