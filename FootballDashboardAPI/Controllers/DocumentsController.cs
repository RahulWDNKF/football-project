using FootballDashboardAPI.Models;
using FootballDashboardAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballDashboardAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class DocumentsController : ControllerBase
{
    private readonly IDocumentService _service;

    public DocumentsController(IDocumentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    //[HttpGet("{id}")]
    //public async Task<IActionResult> Get(string id)
    //{
    //    var doc = await _service.GetByIdAsync(id);

    //    if (doc == null)
    //        return NotFound();

    //    return Ok(doc);
    //}

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDocument dto)
    {
        var doc = await _service.CreateAsync(dto);
        return Ok(doc);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdateDocument dto)
    {
        var result = await _service.UpdateAsync(id, dto);

        if (!result)
            return NotFound();

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _service.DeleteAsync(id);

        if (!result)
            return NotFound();

        return NoContent();
    }
}