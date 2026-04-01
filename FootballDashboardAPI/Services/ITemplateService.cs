using FootballDashboardAPI.Models;

namespace FootballDashboardAPI.Services;

public interface ITemplateService
{
    Task<IEnumerable<Template>> GetAllTemplatesAsync();
    Task<Template?> GetTemplateByIdAsync(string id);
    Task<Template> CreateTemplateAsync(CreateTemplate createTemplateDto);
    Task<Template?> UpdateTemplateAsync(string id, UpdateTemplate updateTemplateDto);
    Task<bool> DeleteTemplateAsync(string id);
}
