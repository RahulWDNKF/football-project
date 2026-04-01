using FootballDashboardAPI.Models;

namespace FootballDashboardAPI.Services;

public interface IClubService
{
    Task<IEnumerable<Club>> GetAllClubsAsync();
    Task<Club?> GetClubByIdAsync(string id);
    Task<Club> CreateClubAsync(CreateClub createClubDto);
    Task<Club?> UpdateClubAsync(string id, UpdateClub updateClubDto);
    Task<bool> DeleteClubAsync(string id);
}