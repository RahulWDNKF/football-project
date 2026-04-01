using FootballDashboardAPI.Models;
using Task = FootballDashboardAPI.Models.Task;

namespace FootballDashboardAPI.Services;

public interface ITaskService
{
    Task<IEnumerable<Task>> GetAllTasksAsync();
    Task<Task?> GetTaskByIdAsync(string id);
    Task<Task> CreateTaskAsync(CreateTask dto);
    Task<Task?> UpdateTaskAsync(string id, UpdateTask dto);
    Task<bool> DeleteTaskAsync(string id);
}