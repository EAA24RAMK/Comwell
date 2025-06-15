using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using WebApp;
using WebApp.Services;
using WebApp.Services.Export;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Tilføjer Blazored LocalStorage service
// Giver mulighed for at gemme og hente data fra browserens localStorage (fx loggedInUser)
builder.Services.AddBlazoredLocalStorage();

// Tilføjer authentication services
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();

// Dependency Injection – registrerer services til hele appen
// Når en komponent fx beder om ILoginService, får den en instans af LoginService
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentPlanService, StudentPlanService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ILearningMaterialService, LearningMaterialService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IExportService, ExportService>();

// Konfigurerer HttpClient til at sende requests til backend
// BaseAddress: Læser API URL fra appsettings.json baseret på environment
builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient();
    var config = builder.Configuration;
    
    // Forsøger at læse fra appsettings.json, fallback til localhost hvis ikke fundet
    var apiBaseUrl = config["ApiSettings:BaseUrl"];
    
    // Hvis ikke fundet i config, brug Azure URL som default i stedet for localhost
    if (string.IsNullOrEmpty(apiBaseUrl))
    {
        apiBaseUrl = "https://comwelltpapi.azurewebsites.net/";
    }
    
    // Validerer at URL'en er gyldig
    if (!Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var uri))
    {
        throw new ArgumentException($"Invalid API base URL: {apiBaseUrl}");
    }
    
    httpClient.BaseAddress = uri;
    return httpClient;
});

await builder.Build().RunAsync();