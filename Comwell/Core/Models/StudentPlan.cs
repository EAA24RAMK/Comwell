namespace Core.Models;

public class StudentPlan
{
    public string? PlanId { get; set; }
    public string Title { get; set; } = "";
    public string StudentId { get; set; } = "";
    public string TemplateId { get; set; } = "";
    public string CreatedBy { get; set; } = "";
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}