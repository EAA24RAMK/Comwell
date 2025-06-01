using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApp;
using WebApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Tilføjer Blazored LocalStorage service
// Giver mulighed for at gemme og hente data fra browserens localStorage (fx loggedInUser)
builder.Services.AddBlazoredLocalStorage();

// Dependency Injection – registrerer services til hele appen
// Når en komponent fx beder om ILoginService, får den en instans af LoginService
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentPlanService, StudentPlanService>();
builder.Services.AddScoped<ITemplateService, TemplateService>();
builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<ILearningMaterialService, LearningMaterialService>();

// Konfigurerer HttpClient til at sende requests til backend
// BaseAddress: Angiver hvilket API vi arbejder imod 
builder.Services.AddScoped(sp =>
    new HttpClient { BaseAddress = new Uri("https://comwelltpapi.azurewebsites.net/") }
);

await builder.Build().RunAsync();