using Core.Models;

namespace ServerAPI.Repositories;

public interface IStudentPlanRepository
{
    Task CreateStudentPlanAsync(StudentPlan createPlan);
    Task<List<StudentPlan>> GetAllPlansAsync();
}