using Core.Models;

namespace ServerAPI.Repositories;

public interface ITemplateRepository
{
    Task CreateStandardTemplateAsync();
    Task<List<Template>> GetAllTemplatesAsync();
    Task<Template?> GetTemplateByIdAsync(int id);
    Task UpdateTemplateAsync(Template template);
}