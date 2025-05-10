using Core.Models;

namespace ServerAPI.Repositories;

public interface ITemplateRepository
{
    Task CreateStandardTemplateAsync();
    Task<List<Template>> GetAllTemplatesAsync();
    Task<Template?> GetTemplateByIdAsync(int id);
    Task AddTemplateAsync(Template template);
    Task UpdateTemplateAsync(Template template);
    Task DeleteTemplateAsync(int id);
}