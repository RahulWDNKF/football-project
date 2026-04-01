using FootballDashboardAPI.Models;
using FootballDashboardAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballDashboardAPI.Controllers;
[AllowAnonymous]

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayersController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
    {
        var players = await _playerService.GetAllPlayersAsync();
        return Ok(players);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Player>> GetPlayer(long id)
    {
        var player = await _playerService.GetPlayerByIdAsync(id);
        
        if (player == null)
        {
            return NotFound(new { message = $"Player with ID '{id}' not found." });
        }

        return Ok(player);
    }

    [HttpPost]
    public async Task<ActionResult<Player>> CreatePlayer(CreatePlayer createPlayerDto)
    {
        var player = await _playerService.CreatePlayerAsync(createPlayerDto);
        return CreatedAtAction(nameof(GetPlayer), new { id = player.Id }, player);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Player>> UpdatePlayer(long id, UpdatePlayer updatePlayerDto)
    {
        var player = await _playerService.UpdatePlayerAsync(id, updatePlayerDto);
        
        if (player == null)
        {
            return NotFound(new { message = $"Player with ID '{id}' not found." });
        }

        return Ok(player);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePlayer(long id)
    {
        var result = await _playerService.DeletePlayerAsync(id);
        
        if (!result)
        {
            return NotFound(new { message = $"Player with ID '{id}' not found." });
        }

        return NoContent();
    }

    [HttpPost("upload-image/{playerId}")]
    public async Task<IActionResult> UploadPlayerImage(long playerId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded");

        // Folder path
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "player-images");

        // Create folder if not exists
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // STEP 1: Delete existing file (any extension)
        var existingFiles = Directory.GetFiles(folderPath, $"player-{playerId}.*");

        foreach (var existingFile in existingFiles)
        {
            System.IO.File.Delete(existingFile);
        }

        // STEP 2: Save new file
        var extension = Path.GetExtension(file.FileName).ToLower();
        var fileName = $"player-{playerId}{extension}";
        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // STEP 3: Generate URL
        var imageUrl = $"{Request.Scheme}://{Request.Host}/player-images/{fileName}";

        return Ok(new { imageUrl });
    }
}
