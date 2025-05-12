using Core.Models;
using MongoDB.Driver;

namespace ServerAPI.Repositories;

public class TemplateRepository : ITemplateRepository
{
    private readonly IMongoCollection<Template> _templates;
    public TemplateRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _templates = db.GetCollection<Template>("template");
    }

    // Opretter skabelon til opret plan
    public async Task CreateStandardTemplateAsync()
    {
         var existing = await _templates.Find(t => t.Id == 1).FirstOrDefaultAsync();
         if (existing != null) return;

         var template = new Template
        {
            Id = 1,
            Title = "Standard kokkeelev",
            CreatedBy = "System",
            Goals = new List<Goal>
            {
                new Goal
                {
                    Id = 1,
                    Title = "1. skoleperiode",
                    Description = "Første skoleperiode – grundlæggende teori og praktiske færdigheder.",
                    Category = "Skole",
                    Deadline = DateTime.UtcNow.AddDays(10 * 7),
                    Status = "Ikke startet",
                    Responsible = "",
                    InitiatedBy = "HR",
                    CheckedOff = false,
                    Subtasks = new List<string>
                    {
                        "Introduktion til madlavning",
                        "Basale råvarer og ernæring",
                        "Teori om hygiejne og fødevaresikkerhed"
                    },
                    Comments = new List<string>(),
                    StudentNotes = new List<string>(),
                    Attachments = new List<string>()
                },
                new Goal
                {
                    Id = 2,
                    Title = "1. praktikperiode",
                    Description = "Praktisk oplæring i køkkenet – fokus på rutiner og køkkenteknik.",
                    Category = "Praktik",
                    Deadline = DateTime.UtcNow.AddDays((10 + 52) * 7),
                    Status = "Ikke startet",
                    Responsible = "",
                    InitiatedBy = "HR",
                    CheckedOff = false,
                    Subtasks = new List<string>
                    {
                        "Deltagelse i frokostproduktion",
                        "Anvende basis knivteknik",
                        "Rengøring og arbejdsdisciplin"
                    },
                    Comments = new List<string>(),
                    StudentNotes = new List<string>(),
                    Attachments = new List<string>()
                },
                new Goal
                {
                    Id = 3,
                    Title = "2. skoleperiode",
                    Description = "Udbygning af teknikker og specialviden.",
                    Category = "Skole",
                    Deadline = DateTime.UtcNow.AddDays((10 + 52 + 10) * 7),
                    Status = "Ikke startet",
                    Responsible = "",
                    InitiatedBy = "HR",
                    CheckedOff = false,
                    Subtasks = new List<string>
                    {
                        "Menusammensætning",
                        "Fisk og skaldyr",
                        "Avanceret tilberedning"
                    },
                    Comments = new List<string>(),
                    StudentNotes = new List<string>(),
                    Attachments = new List<string>()
                },
                new Goal
                {
                    Id = 4,
                    Title = "2. praktikperiode",
                    Description = "Flere ansvarsområder i køkkenet – selvstændighed og effektivitet.",
                    Category = "Praktik",
                    Deadline = DateTime.UtcNow.AddDays((10 + 52 + 10 + 42) * 7),
                    Status = "Ikke startet",
                    Responsible = "",
                    InitiatedBy = "HR",
                    CheckedOff = false,
                    Subtasks = new List<string>
                    {
                        "Planlægge og udføre retter",
                        "Indgå i team og vagtskema",
                        "Deltage i temaaftener eller events"
                    },
                    Comments = new List<string>(),
                    StudentNotes = new List<string>(),
                    Attachments = new List<string>()
                },
                new Goal
                {
                    Id = 5,
                    Title = "3. skoleperiode",
                    Description = "Afsluttende skoleperiode – fokus på eksamen og detaljer.",
                    Category = "Skole",
                    Deadline = DateTime.UtcNow.AddDays((10 + 52 + 10 + 42 + 7) * 7),
                    Status = "Ikke startet",
                    Responsible = "",
                    InitiatedBy = "HR",
                    CheckedOff = false,
                    Subtasks = new List<string>
                    {
                        "Eksamenstræning",
                        "Specialretter",
                        "Madspild og bæredygtighed"
                    },
                    Comments = new List<string>(),
                    StudentNotes = new List<string>(),
                    Attachments = new List<string>()
                },
                new Goal
                {
                    Id = 6,
                    Title = "3. praktikperiode",
                    Description = "Sidste praktikperiode – eleven arbejder næsten selvstændigt.",
                    Category = "Praktik",
                    Deadline = DateTime.UtcNow.AddDays((10 + 52 + 10 + 42 + 7 + 43) * 7),
                    Status = "Ikke startet",
                    Responsible = "",
                    InitiatedBy = "HR",
                    CheckedOff = false,
                    Subtasks = new List<string>
                    {
                        "Planlægge egne opgaver",
                        "Deltage i evaluering af elever",
                        "Forberede sig til afsluttende prøve"
                    },
                    Comments = new List<string>(),
                    StudentNotes = new List<string>(),
                    Attachments = new List<string>()
                }
            } 
        }; 
         
        await _templates.InsertOneAsync(template);
    }

    // Henter alle skabelon og returner dem i en liste
    public async Task<List<Template>> GetAllTemplatesAsync()
    {
        return await _templates.Find(template => true).ToListAsync();
    }

    // Henter en skabelon efter ID
    public async Task<Template?> GetTemplateByIdAsync(int id)
    {
        return await _templates.Find(template => template.Id == id).FirstOrDefaultAsync();
    }
    
    public async Task UpdateTemplateAsync(Template template)
    {
        throw new NotImplementedException();
    }
}