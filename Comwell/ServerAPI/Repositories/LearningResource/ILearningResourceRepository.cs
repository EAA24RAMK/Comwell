using Core.Models;

namespace ServerAPI.Repositories;

public interface ILearningResourceRepository
{
    void Create(LearningResource resource);
    void Delete(int id);
    List<LearningResource> GetAll();
    LearningResource GetById(int id);
}