using Core.Models;

namespace ServerAPI.Repositories;

public interface IUserAchievementRepository
{
    Task<List<UserAchievement>> GetAllAsync();
    Task<UserAchievement?> GetByIdAsync(int id);
    Task<UserAchievement?> CreateAsync(UserAchievement userAchievement);
    Task<List<UserAchievement>> GetByUserIdAsync(int userId);
    Task<bool> HasUserAchievementAsync(int userId, int achievementId);
} 