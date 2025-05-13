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