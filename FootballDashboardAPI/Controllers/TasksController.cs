using FootballDashboardAPI.Models;
using FootballDashboardAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task = FootballDashboardAPI.Models.Task;

namespace FootballDashboardAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Task>>> GetTasks()
    {
        var tasks = await _taskService.GetAllTasksAsync();
        return Ok(tasks);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Task>> GetTask(string id)
    {
        var task = await _taskService.GetTaskByIdAsync(id);

        if (task == null)
            return NotFound(new { message = $"Task with ID '{id}' not found." });

        return Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<Task>> CreateTask(CreateTask dto)
    {
        if (dto == null)
            return BadRequest(new { message = "Request body is required." });

        var task = await _taskService.CreateTaskAsync(dto);

        return CreatedAtAction(nameof(GetTask), new { id = task.TaskId }, task);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Task>> UpdateTask(string id, UpdateTask dto)
    {
        var task = await _taskService.UpdateTaskAsync(id, dto);

        if (task == null)
            return NotFound(new { message = $"Task with ID '{id}' not found." });

        return Ok(task);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTask(string id)
    {
        var result = await _taskService.DeleteTaskAsync(id);

        if (!result)
            return NotFound(new { message = $"Task with ID '{id}' not found." });

        return NoContent();
    }
}