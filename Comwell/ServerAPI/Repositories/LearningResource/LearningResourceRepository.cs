using Core.Models;
using MongoDB.Driver;

namespace ServerAPI.Repositories;

public class LearningResourceRepository : ILearningResourceRepository
{
    private readonly IMongoCollection<LearningResource> _collection;

    public LearningResourceRepository(IConfiguration config)
    {
        var client = new MongoClient(config.GetConnectionString("DefaultConnection"));
        var database = client.GetDatabase("Comwell");
        _collection = database.GetCollection<LearningResource>("LearningResources");
    }

    public void Create(LearningResource resource)
    {
        // Automatisk generering af næste ID (int)
        var latest = _collection.Find(_ => true)
            .SortByDescending(r => r.Id)
            .Limit(1)
            .FirstOrDefault();

        resource.Id = (latest?.Id ?? 0) + 1;

        _collection.InsertOne(resource);
    }

    public void Delete(int id)
    {
        _collection.DeleteOne(r => r.Id == id);
    }

    public List<LearningResource> GetAll()
    {
        return _collection.Find(_ => true).ToList();
    }

    public LearningResource GetById(int id)
    {
        return _collection.Find(r => r.Id == id).FirstOrDefault();
    }
}