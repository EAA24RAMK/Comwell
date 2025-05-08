using System.Threading.Tasks;
using Core.Models;

namespace WebApp.Services;

public interface IStudentPlanService
{
    Task CreateStudentPlanAsync(StudentPlan plan);
}