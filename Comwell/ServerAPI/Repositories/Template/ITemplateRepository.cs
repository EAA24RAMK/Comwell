using Core.Models;

namespace ServerAPI.Repositories;

public interface ITemplateRepository
{
    Task<List<Template>> GetAllTemplatesAsync();
    Task<Template?> GetTemplateByIdAsync(int id);
}