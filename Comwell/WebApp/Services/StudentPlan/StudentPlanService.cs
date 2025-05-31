using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Core.Models;

namespace WebApp.Services;

/* Service overblik
- Bruges i frontend (WebApp) til at kommunikere med backendens StudentPlanController.
- Sender HTTP-requests for at oprette, hente, opdatere og slette elevplaner.
- Gør det nemt for Blazor-sider at arbejde med elevplan-data. */
public class StudentPlanService : IStudentPlanService
{
    // Instansvariabel til at sende HTTP-anmodninger til backendens API
    private readonly HttpClient _http;
    
    // Konstruktøren hvor HttpClient bliver injected.
    public StudentPlanService(HttpClient http)
    {
        _http = http;
    }
    
    // Parametre: plan – Elevplanen der skal oprettes.
    // Formål: Bruges til at sende en ny elevplan til API'et via en POST-request.
    public async Task CreateStudentPlanAsync(StudentPlan plan)
    {
        await _http.PostAsJsonAsync("api/studentplan", plan);
    }

    // Returnerer: En liste med alle elevplaner fra backend.
    // Formål: Bruges til at hente alle elevplaner, fx til HR-oversigter.
    // ??: Opretter tom liste hvis der ingen studenplans er fundet i databasen.
    public async Task<List<StudentPlan>> GetAllPlansAsync()
    {
        return await _http.GetFromJsonAsync<List<StudentPlan>>("api/studentplan") ?? new(); 
    }
    
    // Returnerer: En liste med elevplaner baseret på brugerens rolle.
    // Parametre: user – Brugerobjekt, som bruges til at afgøre hvilke planer der skal hentes.
    // Formål: Henter elevplaner baseret på brugerens rolle:
    // - HR: får alle planer
    // - Køkkenchef/Kok: får planer for eget hotel
    // - Elev: får kun egne planer
    public async Task<List<StudentPlan>> GetPlansByUserAsync(User user)
    {
        if (user.Role == "HR")
        {
            return await _http.GetFromJsonAsync<List<StudentPlan>>("api/studentplan") ?? new();
        }
        if (user.Role == "Køkkenchef" || user.Role == "Kok")
        {
            return await _http.GetFromJsonAsync<List<StudentPlan>>($"api/studentplan/hotel/{user.Hotel}") ?? new();
        }
        if (user.Role == "Elev")
        {
            return await _http.GetFromJsonAsync<List<StudentPlan>>($"api/studentplan/student/{user.Id}") ?? new();
        }

        return new(); // Returnerer tom liste hvis rollen ikke passer
    }
    
    // Parametre: plan – Den elevplan der skal opdateres.
    // Formål: Bruges til at sende en opdateret elevplan til API'et via en PUT-request.
    public async Task UpdateStudentPlanAsync(StudentPlan plan)
    {
        await _http.PutAsJsonAsync($"api/studentplan/{plan.Id}", plan);
    }
    
    // Returnerer: True hvis planen blev slettet, ellers false.
    // Parametre: id – ID på den elevplan der skal slettes.
    // Formål: Bruges til at slette en elevplan via en DELETE-request til API'et.
    public async Task<bool> DeleteStudentPlanAsync(int id)
    {
        var response = await _http.DeleteAsync($"api/studentplan/{id}");
        return response.IsSuccessStatusCode;
    }
    
    // Parametre: planId – ID på den elevplan der skal godkendes af køkkenchef.
    // Formål: Bruges til at markere en elevplan som godkendt via en PUT-request til API'et.
    public async Task ApprovePlanByChefAsync(int planId)
    {
        await _http.PutAsync($"api/studentplan/{planId}/approve-by-chef", null);
    }
}