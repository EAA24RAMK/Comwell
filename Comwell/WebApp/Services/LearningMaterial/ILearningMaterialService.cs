using Core.Models;

namespace WebApp.Services;

public interface ILearningMaterialService
{
    Task<LearningMaterial?> UploadFileAsync(Stream fileStream, string fileName, string title, int subtaskId);
    Task<LearningMaterial?> AddLinkAsync(LearningMaterial link);
    Task<bool> DeleteAsync(int id);
    Task<List<LearningMaterial>> GetAllAsync();
}