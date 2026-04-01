using FootballDashboardAPI.Models;
using FootballDashboardAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FootballDashboardAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailNotificationService _emailService;
    private readonly IConfiguration _config;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailNotificationService emailService,
        IConfiguration config)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _config = config;
    }

    // ── POST /api/auth/invite-user ────────────────────────────────────────────
    [HttpPost("invite-user")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> InviteUser([FromBody] InviteUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Check for duplicate email
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing != null)
        {
            if (existing.IsInviteAccepted)
                return Conflict(new { message = $"A user with email '{request.Email}' already exists." });

            // Re-invite an existing user whose invitation is still pending.
            existing.FullName = request.FullName;
            existing.Role = request.Role;
            existing.IsActive = false;
            existing.IsInviteAccepted = false;
            existing.InviteToken = GenerateInviteToken();
            existing.InviteTokenExpiry = DateTime.UtcNow.AddHours(48);
            existing.EmailConfirmed = false;

            var updateResult = await _userManager.UpdateAsync(existing);
            if (!updateResult.Succeeded)
                return BadRequest(new { message = "Failed to update existing pending invite.", errors = updateResult.Errors });

            // Ensure Identity role membership matches requested role.
            var currentRoles = await _userManager.GetRolesAsync(existing);
            if (currentRoles.Any())
            {
                var removeRolesResult = await _userManager.RemoveFromRolesAsync(existing, currentRoles);
                if (!removeRolesResult.Succeeded)
                    return BadRequest(new { message = "Failed to update user role.", errors = removeRolesResult.Errors });
            }

            var addRoleResult = await _userManager.AddToRoleAsync(existing, request.Role);
            if (!addRoleResult.Succeeded)
                return BadRequest(new { message = "Failed to assign role to user.", errors = addRoleResult.Errors });

            await SendInviteEmailAsync(request.Email, request.FullName, request.Role, existing.InviteToken!);
            return Ok(new { message = $"Invitation resent to {request.Email}." });
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FullName = request.FullName,
            Role = request.Role,
            IsActive = false,
            IsInviteAccepted = false,
            InviteToken = GenerateInviteToken(),
            InviteTokenExpiry = DateTime.UtcNow.AddHours(48),
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new { message = "Failed to create user.", errors = result.Errors });

        // Add to Identity role
        var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
        if (!roleResult.Succeeded)
            return BadRequest(new { message = "Failed to assign role to user.", errors = roleResult.Errors });

        await SendInviteEmailAsync(request.Email, request.FullName, request.Role, user.InviteToken!);

        return Ok(new { message = $"Invitation sent to {request.Email}." });
    }

    // ── POST /api/auth/accept-invite ─────────────────────────────────────────
    [HttpPost("accept-invite")]
    [AllowAnonymous]
    public async Task<IActionResult> AcceptInvite([FromBody] AcceptInviteRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Find user by token
        var user = _userManager.Users
            .FirstOrDefault(u => u.InviteToken == request.InviteToken);

        if (user == null)
            return BadRequest(new { message = "Invalid or expired invitation token." });

        if (user.IsInviteAccepted)
            return BadRequest(new { message = "This invitation has already been used." });

        if (user.InviteTokenExpiry == null || user.InviteTokenExpiry < DateTime.UtcNow)
            return BadRequest(new { message = "The invitation link has expired. Please request a new invite." });

        // Set password
        var addPasswordResult = await _userManager.AddPasswordAsync(user, request.Password);
        if (!addPasswordResult.Succeeded)
            return BadRequest(new { message = "Password setup failed.", errors = addPasswordResult.Errors });

        // Mark invite accepted
        user.IsInviteAccepted = true;
        user.IsActive = true;
        user.InviteToken = null;
        user.InviteTokenExpiry = null;
        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);

        var token = GenerateJwtToken(user);
        return Ok(new AuthResponse
        {
            Token = token,
            User = MapToDto(user)
        });
    }

    // ── POST /api/auth/login ─────────────────────────────────────────────────
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized(new { message = "Invalid email or password." });

        if (!user.IsActive || !user.IsInviteAccepted)
            return Unauthorized(new { message = "Your account is not yet activated. Please accept your invitation first." });

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (!result.Succeeded)
        {
            if (result.IsLockedOut)
                return Unauthorized(new { message = "Account is temporarily locked due to too many failed attempts." });
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var token = GenerateJwtToken(user);
        return Ok(new AuthResponse
        {
            Token = token,
            User = MapToDto(user)
        });
    }

    // ── GET /api/auth/me ─────────────────────────────────────────────────────
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null || !user.IsActive)
            return Unauthorized(new { message = "User not found or inactive." });

        return Ok(MapToDto(user));
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private string GenerateJwtToken(ApplicationUser user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expiryHours = _config.GetValue<int>("Jwt:ExpiryHours", 8);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("fullName", user.FullName),
            new Claim("role", user.Role),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expiryHours),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateInviteToken()
    {
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(tokenBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }

    private async System.Threading.Tasks.Task SendInviteEmailAsync(string email, string fullName, string role, string inviteToken)
    {
        var frontendUrl = _config["FrontendUrl"] ?? "http://localhost:5173";
        var inviteLink = $"{frontendUrl}/accept-invite?token={Uri.EscapeDataString(inviteToken)}";
        var emailHtml = BuildInviteEmailHtml(fullName, role, inviteLink);

        try
        {
            await _emailService.SendEmailAsync(
                email,
                fullName,
                "You've been invited to Football Dashboard",
                emailHtml);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Invitation email could not be sent to '{email}'. {ex.Message}", ex);
        }
    }

    private static UserInfoDto MapToDto(ApplicationUser user) => new()
    {
        Id = user.Id,
        Email = user.Email!,
        FullName = user.FullName,
        Role = user.Role
    };

    private static string BuildInviteEmailHtml(string fullName, string role, string inviteLink) => $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f5; margin: 0; padding: 20px;'>
  <div style='max-width: 600px; margin: auto; background: white; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 8px rgba(0,0,0,0.1);'>
    <div style='background: #1d4ed8; padding: 24px 32px;'>
      <h1 style='color: white; margin: 0; font-size: 22px;'>⚽ Football Dashboard</h1>
    </div>
    <div style='padding: 32px;'>
      <h2 style='margin-top: 0; color: #111827;'>You're Invited!</h2>
      <p style='color: #374151; font-size: 15px;'>Hi <strong>{fullName}</strong>,</p>
      <p style='color: #374151; font-size: 15px;'>
        You have been invited to join the <strong>Football Scout Dashboard</strong> as a
        <strong style='color: #1d4ed8;'>{role}</strong>.
      </p>
      <p style='color: #374151; font-size: 15px;'>
        Click the button below to set your password and activate your account:
      </p>
      <div style='text-align: center; margin: 32px 0;'>
        <a href='{inviteLink}'
           style='background: #1d4ed8; color: white; text-decoration: none; padding: 14px 32px;
                  border-radius: 6px; font-size: 16px; font-weight: bold; display: inline-block;'>
          Accept Invitation
        </a>
      </div>
      <p style='color: #6b7280; font-size: 13px;'>
        Or copy this link into your browser:<br/>
        <a href='{inviteLink}' style='color: #1d4ed8; word-break: break-all;'>{inviteLink}</a>
      </p>
      <hr style='border: none; border-top: 1px solid #e5e7eb; margin: 24px 0;'/>
      <p style='color: #9ca3af; font-size: 12px; margin: 0;'>
        ⚠️ This invitation link expires in <strong>48 hours</strong>. If you did not expect this email, you can safely ignore it.
      </p>
    </div>
  </div>
</body>
</html>";
}
