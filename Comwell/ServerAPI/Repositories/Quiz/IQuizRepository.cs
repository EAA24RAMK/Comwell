using Core.Models;

namespace ServerAPI.Repositories;

public interface IQuizRepository
{
    Task<List<Quiz>> GetAllAsync();
    Task<Quiz?> GetByIdAsync(int id);
    Task<Quiz?> CreateAsync(Quiz quiz);
    Task<Quiz?> UpdateAsync(Quiz quiz);
    Task<bool> DeleteAsync(int id);
    Task<List<Quiz>> GetActiveQuizzesAsync();
} 