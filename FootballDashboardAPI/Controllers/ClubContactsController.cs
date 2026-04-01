using FootballDashboardAPI.Models;
using FootballDashboardAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballDashboardAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ClubContactsController : ControllerBase
{
    private readonly IClubContactService _service;

    public ClubContactsController(IClubContactService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClubContact>>> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ClubContact>> Get(string id)
    {
        var item = await _service.GetByIdAsync(id);

        if (item == null)
            return NotFound(new { message = "ClubContact not found" });

        return Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<ClubContact>> Create(CreateClubContact dto)
    {
        try
        {
            var item = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = item.ClubContactId }, item);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ClubContact>> Update(string id, UpdateClubContact dto)
    {
        try
        {
            var item = await _service.UpdateAsync(id, dto);

            if (item == null)
                return NotFound(new { message = "ClubContact not found" });

            return Ok(item);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _service.DeleteAsync(id);

        if (!result)
            return NotFound(new { message = "ClubContact not found" });

        return NoContent();
    }
}