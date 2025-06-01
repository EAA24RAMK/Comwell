using Core.Models;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;

namespace ServerAPI.Repositories;

// Repository overblik
// - Håndterer læringsmaterialer og filer gemt i MongoDB.
// - Bruges til at hente, oprette, uploade, slette filer og materialer.
// - Arbejder både med en almindelig collection og GridFS til filhåndtering.
public class LearningMaterialRepository : ILearningMaterialRepository
{
    private readonly IMongoCollection<LearningMaterial> _materials; // Collection til læringsmaterialer
    private readonly IGridFSBucket _gridFS;                         // GridFS bucket til filer
    
    // Konstruktøren sørger for at etablere forbindelsen til MongoDB og sætter collections og GridFS op.
    // connection string og databasenavn læses fra appsettings.json.
    public LearningMaterialRepository(IConfiguration config)
    {
        var client = new MongoClient(config["MongoDB:ConnectionString"]);
        var db = client.GetDatabase(config["MongoDB:DatabaseName"]);

        _materials = db.GetCollection<LearningMaterial>("learningmaterial");
        _gridFS = new GridFSBucket(db);
    }

    // Returnerer: Liste med alle læringsmaterialer.
    // Formål: Henter alle læringsmaterialer fra databasen.
    public async Task<List<LearningMaterial>> GetAllAsync()
    {
        return await _materials.Find(_ => true).ToListAsync();
    }

    // Returnerer: Et specifikt læringsmateriale ud fra ID.
    // Parametre: id – ID på læringsmaterialet.
    // Formål: Finder et bestemt læringsmateriale baseret på ID.
    public async Task<LearningMaterial?> GetByIdAsync(int id)
    {
        return await _materials.Find(m => m.Id == id).FirstOrDefaultAsync();
    }

    // Returnerer: Det nyoprettede læringsmateriale.
    // Parametre: material – Læringsmateriale-objekt der skal gemmes.
    // Formål: Opretter et nyt læringsmateriale med unikt ID og oprettelsesdato.
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

    // Returnerer: ID på filen som en string.
    // Parametre:
    //   fileStream – Stream med indholdet af filen.
    //   fileName – Navn på filen.
    // Formål: Lagrer en fil i GridFS og returnerer ID'et på filen.
    public async Task<string> UploadFileAsync(Stream fileStream, string fileName)
    {
        var fileId = await _gridFS.UploadFromStreamAsync(fileName, fileStream);
        return fileId.ToString();
    }

    // Returnerer: En fil som stream.
    // Parametre: fileId – ID på filen der skal hentes.
    // Formål: Henter en fil fra GridFS baseret på dens ID.
    public async Task<Stream> GetFileStreamAsync(string fileId)
    {
        var objectId = ObjectId.Parse(fileId);
        return await _gridFS.OpenDownloadStreamAsync(objectId);
    }

    // Returnerer: True hvis materialet blev slettet, ellers false.
    // Parametre: id – ID på læringsmaterialet.
    // Formål: Sletter læringsmaterialet og tilhørende fil fra GridFS hvis det ikke er et link.
    public async Task<bool> DeleteAsync(int id)
    {
        var material = await GetByIdAsync(id); // Find materialet først
        if (material == null) return false;    // Hvis ikke fundet, gør ingenting

        // Hvis materialet er en fil (ikke et link) og filId findes, slet filen i GridFS
        if (!material.IsLink && !string.IsNullOrEmpty(material.FileId))
        {
            await _gridFS.DeleteAsync(ObjectId.Parse(material.FileId));
        }

        var result = await _materials.DeleteOneAsync(m => m.Id == id); // Slet materialet fra databasen
        return result.DeletedCount > 0; // Returnér true hvis noget blev slettet
    }
}
