using Core.Models;

namespace ServerAPI.Repositories;

public interface ILearningMaterialRepository
{
    Task<LearningMaterial?> GetByIdAsync(int id);
    Task<List<LearningMaterial>> GetAllAsync();
    Task<LearningMaterial?> CreateAsync(LearningMaterial material);
    Task<string> UploadFileAsync(Stream fileStream, string fileName);
    Task<Stream> GetFileStreamAsync(string fileId);
    Task<bool> DeleteAsync(int id);
}