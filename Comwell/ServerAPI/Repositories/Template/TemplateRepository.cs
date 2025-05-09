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
    
    public async Task CreateAsync(Template template)
    {
        int maxId = 0;
        var allTemplates = await _templates.Find(_ => true).ToListAsync();
        if (allTemplates.Any())
        {
            maxId = allTemplates.Max(t => t.Id);
        }

        template.Id = maxId + 1;
        
        await _templates.InsertOneAsync(template);
    }

    public async Task<Template?> GetByIdAsync(int id)
    {
        return await _templates.Find(t => t.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Template>> GetAllAsync()
    {
        return await _templates.Find(_ => true).ToListAsync();
    }
}