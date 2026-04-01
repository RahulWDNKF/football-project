using Microsoft.AspNetCore.Identity;

namespace FootballDashboardAPI.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;

    /// <summary>Admin | Player | Scout</summary>
    public string Role { get; set; } = string.Empty;

    public bool IsActive { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? InviteToken { get; set; }

    public DateTime? InviteTokenExpiry { get; set; }

    public bool IsInviteAccepted { get; set; } = false;
}
