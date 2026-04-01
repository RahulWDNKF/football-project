//using FootballDashboardAPI.DTOs;
using FootballDashboardAPI.Models;
using FootballDashboardAPI.Repositories;
using Microsoft.AspNetCore.Identity;

namespace FootballDashboardAPI.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(IUserRepository userRepository, UserManager<ApplicationUser> userManager)
    {
        _userRepository = userRepository;
        _userManager = userManager;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToDto);
    }

    public async Task<User?> GetUserByIdAsync(long id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null ? null : MapToDto(user);
    }

    public async Task<User> CreateUserAsync(CreateUser createUserDto)
    {
        if (await _userRepository.EmailExistsAsync(createUserDto.Email))
        {
            throw new InvalidOperationException($"User with email '{createUserDto.Email}' already exists.");
        }

        var user = new User
        {
            Name = createUserDto.Name,
            Email = createUserDto.Email,
            Password = createUserDto.Password,
            Role = createUserDto.Role,
            Phone = createUserDto.Phone,
            Status = createUserDto.Status ?? true,
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.CreateAsync(user);
        return MapToDto(createdUser);
    }

    public async Task<User?> UpdateUserAsync(long id, UpdateUser updateUserDto)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);
        if (existingUser == null)
            return null;

        if (await _userRepository.EmailExistsAsync(updateUserDto.Email, id))
        {
            throw new InvalidOperationException($"User with email '{updateUserDto.Email}' already exists.");
        }

        var user = new User
        {
            Id = id,
            Name = updateUserDto.Name,
            Email = updateUserDto.Email,
            Password = existingUser.Password,
            Role = updateUserDto.Role,
            Phone = updateUserDto.Phone,
            Status = updateUserDto.Status ?? true,
            CreatedAt = existingUser.CreatedAt,
            UpdatedAt = DateTime.UtcNow
        };

        var updatedUser = await _userRepository.UpdateAsync(user);
        if (updatedUser != null)
        {
            await SyncAuthUserAsync(existingUser.Email, updatedUser.Name);
        }
        return updatedUser == null ? null : MapToDto(updatedUser);
    }

    public async Task<bool> DeleteUserAsync(long id)
    {
        return await _userRepository.DeleteAsync(id);
    }

    private async System.Threading.Tasks.Task SyncAuthUserAsync(string email, string newName)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(newName))
            return;

        var identityUser = await _userManager.FindByEmailAsync(email);
        if (identityUser == null)
            return;

        if (string.Equals(identityUser.FullName, newName, StringComparison.OrdinalIgnoreCase))
            return;

        identityUser.FullName = newName;
        await _userManager.UpdateAsync(identityUser);
    }

    private static User MapToDto(User user)
    {
        return new User
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role,
            Phone = user.Phone,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
