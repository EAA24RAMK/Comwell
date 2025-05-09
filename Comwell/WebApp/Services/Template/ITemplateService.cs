using Core.Models;

namespace WebApp.Services;

public interface ITemplateService
{
    Task<List<Template>> GetAllTemplatesAsync();
    Task<Template?> GetTemplateByIdAsync(string id);
    Task<bool> CreateTemplateAsync(Template newTemplate);
}