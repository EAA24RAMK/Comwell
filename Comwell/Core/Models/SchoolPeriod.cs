namespace Core.Models;

public class SchoolPeriod
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public int? TemplateId { get; set; }
    public string Status { get; set; } = "Ikke startet";
    public DateTime? StartDate { get; set; }
    public bool IsDateConfirmed { get; set; }
    public int DurationWeeks { get; set; }
    public bool IsApproved { get; set; } = false;
    
}