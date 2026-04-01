using FootballDashboardAPI.Models;
using FootballDashboardAPI.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FootballDashboardAPI.Services;

public class ScoutService : IScoutService
{
    private readonly IScoutRepository _scoutRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public ScoutService(IScoutRepository scoutRepository, UserManager<ApplicationUser> userManager)
    {
        _scoutRepository = scoutRepository;
        _userManager = userManager;
    }

    public async Task<IEnumerable<Scout>> GetAllScoutsAsync()
    {
        var scouts = await _scoutRepository.GetAllAsync();
        return scouts.Select(MapToDto);
    }

    public async Task<Scout?> GetScoutByIdAsync(string id)
    {
        var scout = await _scoutRepository.GetByIdAsync(id);
        return scout == null ? null : MapToDto(scout);
    }

    public async Task<Scout> CreateScoutAsync(CreateScout createScoutDto)
    {
        if (await _scoutRepository.ScoutNameExistsAsync(createScoutDto.ScoutName))
        {
            throw new InvalidOperationException($"Scout with name '{createScoutDto.ScoutName}' already exists.");
        }

        // Get max ID from repository
        var maxId = await _scoutRepository.GetMaxScoutIdAsync();
        int nextNumber = 1;
        if (maxId != null && int.TryParse(maxId, out var num))
        {
            nextNumber = num + 1;
        }

        var scout = new Scout
        {
            ScoutId = $"{nextNumber}",
            ScoutName = createScoutDto.ScoutName,
            RoleName = createScoutDto.RoleName,
            FirstName = createScoutDto.FirstName,
            LastName = createScoutDto.LastName,
            Email = createScoutDto.Email,
            PhoneNumber = createScoutDto.PhoneNumber,
            AddressLine1 = createScoutDto.AddressLine1,
            AddressLine2 = createScoutDto.AddressLine2,
            City = createScoutDto.City,
            State = createScoutDto.State,
            PostalCode = createScoutDto.PostalCode,
            Country = createScoutDto.Country,
            CreatedAt = DateTime.UtcNow
        };

        var createdScout = await _scoutRepository.CreateAsync(scout);
        return MapToDto(createdScout);
    }

    public async Task<Scout?> UpdateScoutAsync(string id, UpdateScout updateScoutDto)
    {
        var existingScout = await _scoutRepository.GetByIdAsync(id);
        if (existingScout == null)
            return null;

        if (await _scoutRepository.ScoutNameExistsAsync(updateScoutDto.ScoutName, id))
        {
            throw new InvalidOperationException($"Scout with name '{updateScoutDto.ScoutName}' already exists.");
        }

        var scout = new Scout
        {
            ScoutId = id,
            ScoutName = updateScoutDto.ScoutName,
            RoleName = updateScoutDto.RoleName,
            FirstName = updateScoutDto.FirstName,
            LastName = updateScoutDto.LastName,
            Email = updateScoutDto.Email,
            PhoneNumber = updateScoutDto.PhoneNumber,
            AddressLine1 = updateScoutDto.AddressLine1,
            AddressLine2 = updateScoutDto.AddressLine2,
            City = updateScoutDto.City,
            State = updateScoutDto.State,
            PostalCode = updateScoutDto.PostalCode,
            Country = updateScoutDto.Country,
            CreatedAt = existingScout.CreatedAt
        };

        var updatedScout = await _scoutRepository.UpdateAsync(scout);
        if (updatedScout != null)
        {
            await SyncAuthUserFullNameAsync(existingScout, updatedScout);
        }
        return updatedScout == null ? null : MapToDto(updatedScout);
    }

    public async Task<bool> DeleteScoutAsync(string id)
    {
        return await _scoutRepository.DeleteAsync(id);
    }

    private static Scout MapToDto(Scout scout)
    {
        return new Scout
        {
            ScoutId = scout.ScoutId,
            ScoutName = scout.ScoutName,
            RoleName = scout.RoleName,
            FirstName = scout.FirstName,
            LastName = scout.LastName,
            Email = scout.Email,
            PhoneNumber = scout.PhoneNumber,
            AddressLine1 = scout.AddressLine1,
            AddressLine2 = scout.AddressLine2,
            City = scout.City,
            State = scout.State,
            PostalCode = scout.PostalCode,
            Country = scout.Country,
            CreatedAt = scout.CreatedAt
        };
    }

    private async System.Threading.Tasks.Task SyncAuthUserFullNameAsync(Scout previousScout, Scout updatedScout)
    {
        ApplicationUser? identityUser = null;

        if (!string.IsNullOrWhiteSpace(updatedScout.Email))
        {
            identityUser = await _userManager.FindByEmailAsync(updatedScout.Email);
        }

        if (identityUser == null && !string.IsNullOrWhiteSpace(previousScout.Email))
        {
            identityUser = await _userManager.FindByEmailAsync(previousScout.Email);
        }

        if (identityUser == null)
        {
            var matchedUsers = await _userManager.Users
                .Where(u => u.Role == "Scout" && u.FullName == previousScout.ScoutName)
                .ToListAsync();
            if (matchedUsers.Count == 1)
            {
                identityUser = matchedUsers[0];
            }
        }

        if (identityUser == null)
        {
            return;
        }

        if (string.Equals(identityUser.FullName, updatedScout.ScoutName, StringComparison.Ordinal))
        {
            return;
        }

        identityUser.FullName = updatedScout.ScoutName;
        await _userManager.UpdateAsync(identityUser);
    }
}
