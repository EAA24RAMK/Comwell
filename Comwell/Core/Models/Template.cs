using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Models;

public class Template
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    
    [BsonElement("title")]
    public string Title { get; set; } = "";
    
    [BsonElement("createdBy")]
    public string CreatedBy { get; set; } = "";
    
    [BsonElement("sections")]
    public List<TemplateSection> Sections { get; set; } = new ();

    public class TemplateSection
    {
        [BsonElement("title")]
        public string Title { get; set; } = "";
        [BsonElement("daysAfterStart")]
        public int DaysAfterStart { get; set; } = 0;
    }
}