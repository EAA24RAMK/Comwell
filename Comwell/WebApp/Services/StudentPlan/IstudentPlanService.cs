using System.Threading.Tasks;
using Core.Models;

namespace WebApp.Services;

public interface IStudentPlanService
{
    Task CreateStudentPlanAsync(StudentPlan plan);
    Task<List<StudentPlan>> GetAllPlansAsync();
    
    // Henter planer for den user der er logget ind
    Task<List<StudentPlan>> GetPlansByUserAsync(User user);

    Task UpdateStudentPlanAsync(StudentPlan plan);

}