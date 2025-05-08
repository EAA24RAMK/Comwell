using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Core.Models;

namespace WebApp.Services;

public class StudentPlanService : IStudentPlanService
{
    private readonly HttpClient _http;
    public StudentPlanService(HttpClient http)
    {
        _http = http;
    }
    
    public async Task CreateStudentPlanAsync(StudentPlan plan)
    {
        await _http.PostAsJsonAsync("api/studentplan", plan);
    }
}