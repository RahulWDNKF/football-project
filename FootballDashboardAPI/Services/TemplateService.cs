using FootballDashboardAPI.Models;
using FootballDashboardAPI.Repositories;

namespace FootballDashboardAPI.Services;

public class TemplateService : ITemplateService
{
    private readonly ITemplateRepository _templateRepository;

    public TemplateService(ITemplateRepository templateRepository)
    {
        _templateRepository = templateRepository;
    }

    public async Task<IEnumerable<Template>> GetAllTemplatesAsync()
    {
        var templates = await _templateRepository.GetAllAsync();
        return templates.Select(MapToDto);
    }

    public async Task<Template?> GetTemplateByIdAsync(string id)
    {
        var template = await _templateRepository.GetByIdAsync(id);
        return template == null ? null : MapToDto(template);
    }

    public async Task<Template> CreateTemplateAsync(CreateTemplate createTemplateDto)
    {
        if (await _templateRepository.TemplateNameExistsAsync(createTemplateDto.TemplateName))
        {
            throw new InvalidOperationException($"Template with name '{createTemplateDto.TemplateName}' already exists.");
        }

        var template = new Template
        {
            //TemplateId = Guid.NewGuid().ToString(),
            TemplateId = string.Empty,
            TemplateName = createTemplateDto.TemplateName,
            TemplateType = createTemplateDto.TemplateType,
            Subject = createTemplateDto.Subject,
            Body = createTemplateDto.Body,
            CreatedAt = DateTime.UtcNow
        };

        var createdTemplate = await _templateRepository.CreateAsync(template);
        return MapToDto(createdTemplate);
    }

    public async Task<Template?> UpdateTemplateAsync(string id, UpdateTemplate updateTemplateDto)
    {
        var existingTemplate = await _templateRepository.GetByIdAsync(id);
        if (existingTemplate == null)
            return null;

        if (await _templateRepository.TemplateNameExistsAsync(updateTemplateDto.TemplateName, id))
        {
            throw new InvalidOperationException($"Template with name '{updateTemplateDto.TemplateName}' already exists.");
        }

        var template = new Template
        {
            TemplateId = id,
            TemplateName = updateTemplateDto.TemplateName,
            TemplateType = updateTemplateDto.TemplateType,
            Subject = updateTemplateDto.Subject,
            Body = updateTemplateDto.Body,
            CreatedAt = existingTemplate.CreatedAt
        };

        var updatedTemplate = await _templateRepository.UpdateAsync(template);
        return updatedTemplate == null ? null : MapToDto(updatedTemplate);
    }

    public async Task<bool> DeleteTemplateAsync(string id)
    {
        return await _templateRepository.DeleteAsync(id);
    }

    private static Template MapToDto(Template template)
    {
        return new Template
        {
            TemplateId = template.TemplateId,
            TemplateName = template.TemplateName,
            TemplateType = template.TemplateType,
            Subject = template.Subject,
            Body = template.Body,
            CreatedAt = template.CreatedAt
        };
    }
}
