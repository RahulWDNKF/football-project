using FootballDashboardAPI.Models;

namespace FootballDashboardAPI.Repositories;

public interface ITemplateRepository
{
    Task<IEnumerable<Template>> GetAllAsync();
    Task<Template?> GetByIdAsync(string id);
    Task<Template> CreateAsync(Template template);
    Task<Template?> UpdateAsync(Template template);
    Task<bool> DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task<bool> TemplateNameExistsAsync(string templateName, string? excludeTemplateId = null);
}
