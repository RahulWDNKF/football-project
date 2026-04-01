using FootballDashboardAPI.Models;
using FootballDashboardAPI.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FootballDashboardAPI.Services;

public class PlayerService : IPlayerService
{
    private readonly IPlayerRepository _playerRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public PlayerService(IPlayerRepository playerRepository, UserManager<ApplicationUser> userManager)
    {
        _playerRepository = playerRepository;
        _userManager = userManager;
    }

    public async Task<IEnumerable<Player>> GetAllPlayersAsync()
    {
        var players = await _playerRepository.GetAllAsync();
        return players.Select(MapToDto);
    }

    public async Task<Player?> GetPlayerByIdAsync(long id)   
    {
        var player = await _playerRepository.GetByIdAsync(id);
        return player == null ? null : MapToDto(player);
    }

    public async Task<Player> CreatePlayerAsync(CreatePlayer createPlayerDto)
    {
        var player = new Player1
        {
            //PlayerId = Guid.NewGuid().ToString(),
            FullName = createPlayerDto.FullName,
            DateOfBirth = createPlayerDto.DateOfBirth ?? DateOnly.FromDateTime(DateTime.Now),
            Nationality = createPlayerDto.Nationality ?? string.Empty,
            PositionCode = createPlayerDto.Position ?? string.Empty,
            PreferredFoot = createPlayerDto.PreferredFoot ?? string.Empty,
            HeightCm = createPlayerDto.HeightCm ?? 0,
            WeightKg = createPlayerDto.WeightKg ?? 0,
            CurrentClubId = createPlayerDto.CurrentClub,
            ContractStartDate = createPlayerDto.ContractStart ?? DateOnly.FromDateTime(DateTime.Now),
            ContractEndDate = createPlayerDto.ContractEnd ?? DateOnly.FromDateTime(DateTime.Now.AddYears(1)),
            AgentName = createPlayerDto.AgentName ?? string.Empty,

            //AgentScoutId = Guid.NewGuid().ToString(),
            AgentScoutId = createPlayerDto.AgentScoutId ?? string.Empty,
            ContactInfo = createPlayerDto.ContactInfo ?? string.Empty,
            ProfileImageUrl = createPlayerDto.ProfileImage ?? null,
            playerEmail = createPlayerDto.PlayerEmail ?? string.Empty,

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var createdPlayer = await _playerRepository.CreateAsync(player);
        return MapToDto(createdPlayer);
    }

    public async Task<Player?> UpdatePlayerAsync(long id, UpdatePlayer updatePlayerDto)
    {
        var existingPlayer = await _playerRepository.GetByIdAsync(id);
        if (existingPlayer == null)
            return null;

        var player = new Player1
        {
            PlayerId = existingPlayer.PlayerId,
            FullName = updatePlayerDto.FullName,
            DateOfBirth = updatePlayerDto.DateOfBirth ?? existingPlayer.DateOfBirth,
            Nationality = updatePlayerDto.Nationality ?? existingPlayer.Nationality,
            PositionCode = updatePlayerDto.Position ?? existingPlayer.PositionCode,
            PreferredFoot = updatePlayerDto.PreferredFoot ?? existingPlayer.PreferredFoot,
            HeightCm = updatePlayerDto.HeightCm ?? existingPlayer.HeightCm,
            WeightKg = updatePlayerDto.WeightKg ?? existingPlayer.WeightKg,
            CurrentClubId = updatePlayerDto.CurrentClub ?? existingPlayer.CurrentClubId,
            ContractStartDate = updatePlayerDto.ContractStart ?? existingPlayer.ContractStartDate,
            ContractEndDate = updatePlayerDto.ContractEnd ?? existingPlayer.ContractEndDate,
            AgentName = updatePlayerDto.AgentName ?? existingPlayer.AgentName,
            //AgentScoutId = existingPlayer.AgentScoutId,
            CreatedAt = existingPlayer.CreatedAt,
            UpdatedAt = DateTime.UtcNow,

            AgentScoutId = updatePlayerDto.AgentScoutId ?? existingPlayer.AgentScoutId,
            ContactInfo = updatePlayerDto.ContactInfo ?? existingPlayer.ContactInfo,
            ProfileImageUrl = updatePlayerDto.ProfileImage ?? existingPlayer.ProfileImageUrl,
        };

        var updatedPlayer = await _playerRepository.UpdateAsync(player);
        if (updatedPlayer != null)
        {
            await SyncAuthUserFullNameAsync(existingPlayer, updatedPlayer);
        }
        return updatedPlayer == null ? null : MapToDto(updatedPlayer);
    }

    public async Task<bool> DeletePlayerAsync(long id)
    {
        return await _playerRepository.DeleteAsync(id);
    }

    private static Player MapToDto(Player1 player)
    {
        return new Player
        {
            Id = long.TryParse(player.PlayerId, out var id) ? id : 0,
            FullName = player.FullName,
            DateOfBirth = player.DateOfBirth,
            Nationality = player.Nationality,
            Position = player.PositionCode,
            PreferredFoot = player.PreferredFoot,
            HeightCm = player.HeightCm,
            WeightKg = player.WeightKg,
            CurrentClub = player.CurrentClubId,
            ContractStart = player.ContractStartDate,
            ContractEnd = player.ContractEndDate,
            ContractStatus = null,
            AgentName = player.AgentName,
            CreatedAt = player.CreatedAt,
            UpdatedAt = player.UpdatedAt,

            contact_info = player.ContactInfo,
            agent_scout_id = player.AgentScoutId,
            profileImage = player.ProfileImageUrl,
            PlayerEmail = player.playerEmail,
        };
    }

    private async System.Threading.Tasks.Task SyncAuthUserFullNameAsync(Player1 previousPlayer, Player1 updatedPlayer)
    {
        var previousName = previousPlayer.FullName?.Trim();
        var updatedName = updatedPlayer.FullName?.Trim();

        if (string.IsNullOrWhiteSpace(previousName) || string.IsNullOrWhiteSpace(updatedName))
        {
            return;
        }

        if (string.Equals(previousName, updatedName, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var previousNameLower = previousName.ToLower();
        var matchedUsers = await _userManager.Users
            .Where(u => u.Role == "Player" && u.FullName.ToLower() == previousNameLower)
            .ToListAsync();

        if (matchedUsers.Count != 1)
        {
            return;
        }

        var identityUser = matchedUsers[0];
        identityUser.FullName = updatedName;
        await _userManager.UpdateAsync(identityUser);
    }
}
