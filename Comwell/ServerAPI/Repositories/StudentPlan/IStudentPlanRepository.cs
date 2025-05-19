using Core.Models;

namespace ServerAPI.Repositories;

public interface IStudentPlanRepository
{
    Task CreateStudentPlanAsync(StudentPlan createPlan);
    Task<List<StudentPlan>> GetAllPlansAsync();
    
    // metode: henter en elevs plan
    Task<List<StudentPlan>> GetPlansByStudentAsync(int studentId);
    
    // metode: henter planer efter hotel (køkkenchef)
    Task<List<StudentPlan>> GetPlansByHotelAsync(string hotel);
    
    Task<StudentPlan?> GetPlanByIdAsync(int id);
    Task UpdateStudentPlanAsync(StudentPlan updatedPlan);
    
    Task<bool> DeleteStudentPlanAsync(int id);
}