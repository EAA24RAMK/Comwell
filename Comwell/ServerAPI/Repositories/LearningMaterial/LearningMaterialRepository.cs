using Core.Models;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;

namespace ServerAPI.Repositories;

public class LearningMaterialRepository : ILearningMaterialRepository
{
    private readonly IMongoCollection<LearningMaterial> _materials;
    private readonly IGridFSBucket _gridFS;

    public LearningMaterialRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);

        _materials = db.GetCollection<LearningMaterial>("learningmaterial");
        _gridFS = new GridFSBucket(db);
    }

    public async Task<List<LearningMaterial>> GetAllAsync()
    {
        return await _materials.Find(_ => true).ToListAsync();
    }

    public async Task<LearningMaterial?> GetByIdAsync(int id)
    {
        return await _materials.Find(m => m.Id == id).FirstOrDefaultAsync();
    }

    public async Task<LearningMaterial?> CreateAsync(LearningMaterial material)
    {
        int maxId = 0;
        var allMaterials = await _materials.Find(_ => true).ToListAsync();
        if (allMaterials.Any())
        {
            maxId = allMaterials.Max(m => m.Id);
        }

        material.Id = maxId + 1;
        material.CreatedAt = DateTime.UtcNow;

        await _materials.InsertOneAsync(material);
        return material;
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        var fileId = await _gridFS.UploadFromStreamAsync(fileName, fileStream);
        return fileId.ToString();
    }

    public async Task<Stream> GetFileStreamAsync(string fileId)
    {
        var objectId = ObjectId.Parse(fileId);
        return await _gridFS.OpenDownloadStreamAsync(objectId);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var material = await GetByIdAsync(id);
        if (material == null) return false;

        if (!material.IsLink && !string.IsNullOrEmpty(material.FileId))
        {
            await _gridFS.DeleteAsync(ObjectId.Parse(material.FileId));
        }

        var result = await _materials.DeleteOneAsync(m => m.Id == id);
        return result.DeletedCount > 0;
    }
}
