using Core.Models;

namespace ServerAPI.Services;

public interface ILearningResourceService
{
    void Create(LearningResource resource);
    void Delete(int id);
    List<LearningResource> GetAll();
    LearningResource GetById(int id);
}