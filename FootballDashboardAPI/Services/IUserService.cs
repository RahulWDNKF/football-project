//using FootballDashboardAPI.DTOs;
using FootballDashboardAPI.Models;

namespace FootballDashboardAPI.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(long id);
    Task<User> CreateUserAsync(CreateUser createUserDto);
    Task<User?> UpdateUserAsync(long id, UpdateUser updateUserDto);
    Task<bool> DeleteUserAsync(long id);
}
