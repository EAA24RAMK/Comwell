using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Models;

[BsonIgnoreExtraElements]
public class StudentPlan
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("_id")]
    public string? PlanId { get; set; }

    [BsonElement("title")]
    public string Title { get; set; } = "";

    [BsonElement("studentId")]
    public string StudentId { get; set; } = "";

    [BsonElement("templateId")]
    public string TemplateId { get; set; } = "";

    [BsonElement("createdBy")]
    public string CreatedBy { get; set; } = "";

    [BsonElement("periodStart")]
    public DateTime PeriodStart { get; set; }

    [BsonElement("periodEnd")]
    public DateTime PeriodEnd { get; set; }

    [BsonElement("goals")]
    public List<Goal> Goals { get; set; } = new();

    [BsonElement("notes")]
    public List<Note> Notes { get; set; } = new();
}