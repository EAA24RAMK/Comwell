using ServerAPI.Repositories;
using ServerAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Dependency Injection – registrerer repositories
// Når en controller fx beder om IUserRepository, får den en instans af UserRepository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStudentPlanRepository, StudentPlanRepository>();
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<IStudentPlanRepository, StudentPlanRepository>();
builder.Services.AddScoped<IPostRepository, PostRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ILearningMaterialRepository, LearningMaterialRepository>();

// Konfigurerer CORS
// Gør det muligt for WebApp (frontend) at snakke med backend
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://comwelltp.azurewebsites.net") // WebApp URL
            .AllowAnyMethod()                       // Tillad alle HTTP-metoder (GET, POST, PUT, DELETE osv.)
            .AllowAnyHeader();                      // Tillad alle headers
    });
});

var app = builder.Build();

if (
    app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();