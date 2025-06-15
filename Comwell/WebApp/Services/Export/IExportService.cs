using Core.Models;

namespace WebApp.Services.Export
{
    public interface IExportService
    {
        Task<byte[]> ExportToExcelAsync(List<User> users, List<StudentPlan> plans, string selectedLocation = "", string selectedGoalTitle = "");
    }
} 