using Core.Models;
using MongoDB.Driver;

namespace ServerAPI.Repositories;

/* Repository overblik
- Er ansvarlig for alt, der har med skabelondata (Template) at gøre.
- Bindeled mellem databasen og resten af systemet.
- Metoder til at hente data fra databasen.
- Repositoryet bliver brugt i vores TemplateController. */
public class TemplateRepository : ITemplateRepository
{
    // Instansvariabel der refererer til Template-collection i MongoDB
    private readonly IMongoCollection<Template> _templates;
    
    // Konstruktøren sørger for at etablere forbindelsen til MongoDB.
    // connection string og databasenavn læses fra appsettings.json.
    public TemplateRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);
        _templates = db.GetCollection<Template>("template");
    }

    // Returnerer: En liste med alle skabeloner i databasen.
    // Formål: Bruges til at hente alle skabeloner fra databasen, fx for at vise dem i opret plan.
    public async Task<List<Template>> GetAllTemplatesAsync()
    {
        return await _templates.Find(template => true).ToListAsync();
    }

    // Returnerer: En skabelon baseret på dens ID, eller null hvis den ikke findes.
    // Parametre: id – skabelonens ID.
    // Formål: Bruges til at hente en specifik skabelon, fx praktikperiode 1.
    public async Task<Template?> GetTemplateByIdAsync(int id)
    {
        return await _templates.Find(template => template.Id == id).FirstOrDefaultAsync();
    }
}