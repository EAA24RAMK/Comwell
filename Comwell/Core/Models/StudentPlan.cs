using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Core.Models;

[BsonIgnoreExtraElements]
public class StudentPlan
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int StudentId { get; set; } 
    public int TemplateId { get; set; }
    public string CreatedBy { get; set; } = "";
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public List<Goal> Goals { get; set; } = new();
    public List<Note> Notes { get; set; } = new();
}