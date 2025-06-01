namespace Core.Models;

public class StudentPlan
{
    public int Id { get; set; }
    public int StudentId { get; set; } // Reference til User
    public int TemplateId { get; set; } // Reference til Template
    public string CreatedBy { get; set; } = "";
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public List<Goal> Goals { get; set; } = new(); // Embedded liste af Goals
    public List<Note> Notes { get; set; } = new(); // Embedded liste af Notes
    public bool IsApprovedByChef { get; set; } = false;
    public List<SchoolPeriod> SchoolPeriods { get; set; } = new(); // Embedded liste af SchoolPeriods
}