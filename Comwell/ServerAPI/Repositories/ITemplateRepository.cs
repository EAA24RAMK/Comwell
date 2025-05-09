using Core.Models;

namespace ServerAPI.Repositories;

public interface ITemplateRepository
{
    Task CreateAsync(Template template);
    Task<List<Template>> GetAllAsync();
    Task<Template?> GetByIdAsync(string id);
}