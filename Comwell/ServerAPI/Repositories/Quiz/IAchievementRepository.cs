using Core.Models;

namespace ServerAPI.Repositories;

public interface IAchievementRepository
{
    Task<List<Achievement>> GetAllAsync();
    Task<Achievement?> GetByIdAsync(int id);
    Task<Achievement?> CreateAsync(Achievement achievement);
    Task<Achievement?> UpdateAsync(Achievement achievement);
    Task<bool> DeleteAsync(int id);
    Task<List<Achievement>> GetActiveAchievementsAsync();
} 