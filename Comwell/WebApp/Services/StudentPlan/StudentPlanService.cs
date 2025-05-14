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

    public async Task<List<StudentPlan>> GetAllPlansAsync()
    {
        return await _http.GetFromJsonAsync<List<StudentPlan>>("api/studentplan") ?? new();
    }
    
    public async Task<List<StudentPlan>> GetPlansByUserAsync(User user)
    {
        if (user.Role == "HR")
        {
            return await _http.GetFromJsonAsync<List<StudentPlan>>("api/studentplan") ?? new();
        }
        else if (user.Role == "Køkkenchef")
        {
            return await _http.GetFromJsonAsync<List<StudentPlan>>($"api/studentplan/hotel/{user.Hotel}") ?? new();
        }
        else if (user.Role == "Elev")
        {
            return await _http.GetFromJsonAsync<List<StudentPlan>>($"api/studentplan/student/{user.Id}") ?? new();
        }
        else if (user.Role == "Afdelingsleder")
        {
            return await _http.GetFromJsonAsync<List<StudentPlan>>("api/studentplan") ?? new();
        }
        return new();
    }
    
    public async Task UpdateStudentPlanAsync(StudentPlan plan)
    {
        await _http.PutAsJsonAsync($"api/studentplan/{plan.Id}", plan);
    }


}