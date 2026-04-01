using FootballDashboardAPI.Models;
using FootballDashboardAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballDashboardAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TemplatesController : ControllerBase
{
    private readonly ITemplateService _templateService;

    public TemplatesController(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Template>>> GetTemplates()
    {
        var templates = await _templateService.GetAllTemplatesAsync();
        return Ok(templates);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Template>> GetTemplate(string id)
    {
        var template = await _templateService.GetTemplateByIdAsync(id);
        
        if (template == null)
        {
            return NotFound(new { message = $"Template with ID '{id}' not found." });
        }

        return Ok(template);
    }

    [HttpPost]
    public async Task<ActionResult<Template>> CreateTemplate(CreateTemplate createTemplateDto)
    {
        try
        {
            var template = await _templateService.CreateTemplateAsync(createTemplateDto);
            return CreatedAtAction(nameof(GetTemplate), new { id = template.TemplateId }, template);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Template>> UpdateTemplate(string id, UpdateTemplate updateTemplateDto)
    {
        try
        {
            var template = await _templateService.UpdateTemplateAsync(id, updateTemplateDto);
            
            if (template == null)
            {
                return NotFound(new { message = $"Template with ID '{id}' not found." });
            }

            return Ok(template);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTemplate(string id)
    {
        var result = await _templateService.DeleteTemplateAsync(id);
        
        if (!result)
        {
            return NotFound(new { message = $"Template with ID '{id}' not found." });
        }

        return NoContent();
    }
}
