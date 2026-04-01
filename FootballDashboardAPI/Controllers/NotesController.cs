using FootballDashboardAPI.Models;
using FootballDashboardAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FootballDashboardAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class NotesController : ControllerBase
{
    private readonly INoteService _noteService;

    public NotesController(INoteService noteService)
    {
        _noteService = noteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Note>>> GetNotes()
    {
        var notes = await _noteService.GetAllNotesAsync();
        return Ok(notes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Note>> GetNote(string id)
    {
        var note = await _noteService.GetNoteByIdAsync(id);
        
        if (note == null)
        {
            return NotFound(new { message = $"Note with ID '{id}' not found." });
        }

        return Ok(note);
    }

    [HttpGet("club/{clubId}")]
    public async Task<ActionResult<IEnumerable<Note>>> GetNotesByClub(string clubId)
    {
        var notes = await _noteService.GetNotesByClubIdAsync(clubId);
        return Ok(notes);
    }

    [HttpGet("player/{playerId}")]
    public async Task<ActionResult<IEnumerable<Note>>> GetNotesByPlayer(string playerId)
    {
        var notes = await _noteService.GetNotesByPlayerIdAsync(playerId);
        return Ok(notes);
    }

    [HttpPost]
    public async Task<ActionResult<Note>> CreateNote(CreateNote createNoteDto)
    {
        var note = await _noteService.CreateNoteAsync(createNoteDto);
        return CreatedAtAction(nameof(GetNote), new { id = note.NoteId }, note);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Note>> UpdateNote(string id, UpdateNote updateNoteDto)
    {
        var note = await _noteService.UpdateNoteAsync(id, updateNoteDto);
        
        if (note == null)
        {
            return NotFound(new { message = $"Note with ID '{id}' not found." });
        }

        return Ok(note);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteNote(string id)
    {
        var result = await _noteService.DeleteNoteAsync(id);
        
        if (!result)
        {
            return NotFound(new { message = $"Note with ID '{id}' not found." });
        }

        return NoContent();
    }
}
