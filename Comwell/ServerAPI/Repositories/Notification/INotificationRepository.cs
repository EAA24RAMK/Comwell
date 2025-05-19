using Core.Models;

namespace ServerAPI.Repositories;

public interface INotificationRepository
{
    Task<List<Notification>> GetAllAsync();
    Task<List<Notification>> GetForUserAsync(int userId);
    Task AddAsync(Notification notification);
    Task UpdateAsync(Notification notification);
    Task DeleteAsync(int id);
    Task DeleteAllForGoalAsync(int planId, int goalId);

    Task CreateNotificationAsync(StudentPlan plan, Goal goal, List<User> allUsers);
}
