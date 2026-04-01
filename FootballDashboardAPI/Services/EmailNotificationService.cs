using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;

namespace FootballDashboardAPI.Services;

public class EmailNotificationService : IEmailNotificationService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailNotificationService> _logger;

    private string TenantId => _config["Graph:TenantId"] ?? string.Empty;
    private string ClientId => _config["Graph:ClientId"] ?? string.Empty;
    private string ClientSecret => _config["Graph:ClientSecret"] ?? string.Empty;
    private string SenderUserId => _config["Graph:SenderUserId"] ?? string.Empty;
    private bool SaveToSentItems => bool.TryParse(_config["Graph:SaveToSentItems"], out var value) && value;

    public EmailNotificationService(
        IConfiguration config,
        ILogger<EmailNotificationService> logger)
    {
        _config = config;
        _logger = logger;
    }

    // ── Core send method ─────────────────────────────────────────────
    public async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlContent)
    {
        try
        {
            ValidateGraphConfiguration();
            var graphClient = CreateGraphClient();

            var message = new Message
            {
                Subject = subject,
                Body = new ItemBody
                {
                    ContentType = BodyType.Html,
                    Content = htmlContent
                },
                ToRecipients = new List<Recipient>
                {
                    new()
                    {
                        EmailAddress = new EmailAddress
                        {
                            Address = toEmail,
                            Name = string.IsNullOrWhiteSpace(toName) ? toEmail : toName
                        }
                    }
                }
            };

            var requestBody = new SendMailPostRequestBody
            {
                Message = message,
                SaveToSentItems = SaveToSentItems
            };

            await graphClient.Users[SenderUserId].SendMail.PostAsync(requestBody);

            _logger.LogInformation("✅ Email sent to {Email} - {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Failed to send email to {Email}", toEmail);
            throw;
        }
    }

    private void ValidateGraphConfiguration()
    {
        if (string.IsNullOrWhiteSpace(TenantId))
            throw new InvalidOperationException("Graph tenant ID is not configured. Set Graph:TenantId in appsettings or environment variables.");

        if (string.IsNullOrWhiteSpace(ClientId))
            throw new InvalidOperationException("Graph client ID is not configured. Set Graph:ClientId in appsettings or environment variables.");

        if (string.IsNullOrWhiteSpace(ClientSecret))
            throw new InvalidOperationException("Graph client secret is not configured. Set Graph:ClientSecret in appsettings or environment variables.");

        if (string.IsNullOrWhiteSpace(SenderUserId))
            throw new InvalidOperationException("Graph sender user is not configured. Set Graph:SenderUserId to the mailbox UPN or user ID that should send the email.");
    }

    private GraphServiceClient CreateGraphClient()
    {
        var credential = new ClientSecretCredential(TenantId, ClientId, ClientSecret);
        return new GraphServiceClient(
            credential,
            new[] { "https://graph.microsoft.com/.default" });
    }

    // ── Contract expiry alert ─────────────────────────────────────────
    public async Task SendContractExpiryAlertAsync(
        string toEmail, string playerName, string contractEndDate, string scoutName)
    {
        var subject = $"⚠️ Contract Expiring Soon — {playerName}";
        var html = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px;'>
            <div style='background: #e74c3c; padding: 15px; border-radius: 8px 8px 0 0;'>
                <h2 style='color: white; margin: 0;'>⚠️ Contract Expiry Alert</h2>
            </div>
            <div style='border: 1px solid #ddd; padding: 20px; border-radius: 0 0 8px 8px;'>
                <p>Hi <strong>{scoutName}</strong>,</p>
                <p>The contract for player <strong>{playerName}</strong> 
                   is expiring on <strong style='color:#e74c3c;'>{contractEndDate}</strong>.</p>
                <p>Please begin renewal discussions as soon as possible.</p>
                <br/>
                <p style='color: #888; font-size: 12px;'>— Football Scout Dashboard</p>
            </div>
        </div>";

        await SendEmailAsync(toEmail, scoutName, subject, html);
    }

    // ── Task due alert ────────────────────────────────────────────────
    public async Task SendTaskDueAlertAsync(
        string toEmail, string taskTitle, string dueDate, string assignedTo)
    {
        var subject = $"📋 Task Due Soon — {taskTitle}";
        var html = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px;'>
            <div style='background: #f39c12; padding: 15px; border-radius: 8px 8px 0 0;'>
                <h2 style='color: white; margin: 0;'>📋 Task Due Alert</h2>
            </div>
            <div style='border: 1px solid #ddd; padding: 20px; border-radius: 0 0 8px 8px;'>
                <p>Hi <strong>{assignedTo}</strong>,</p>
                <p>The following task is due on <strong style='color:#f39c12;'>{dueDate}</strong>:</p>
                <div style='background:#fff8e1; padding:12px; border-left:4px solid #f39c12; margin:10px 0;'>
                    <strong>{taskTitle}</strong>
                </div>
                <p>Please make sure to complete it on time.</p>
                <br/>
                <p style='color: #888; font-size: 12px;'>— Football Scout Dashboard</p>
            </div>
        </div>";

        await SendEmailAsync(toEmail, assignedTo, subject, html);
    }

    // ── Review follow-up reminder ─────────────────────────────────────
    public async Task SendReviewFollowUpAsync(
        string toEmail, string playerName, string skillKey, string followUpDate, string scoutName)
    {
        var subject = $"🔍 Review Follow-Up — {playerName} ({skillKey})";
        var html = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: auto; padding: 20px;'>
            <div style='background: #3498db; padding: 15px; border-radius: 8px 8px 0 0;'>
                <h2 style='color: white; margin: 0;'>🔍 Review Follow-Up Reminder</h2>
            </div>
            <div style='border: 1px solid #ddd; padding: 20px; border-radius: 0 0 8px 8px;'>
                <p>Hi <strong>{scoutName}</strong>,</p>
                <p>You have a follow-up scheduled for 
                   <strong style='color:#3498db;'>{followUpDate}</strong>:</p>
                <ul>
                    <li>Player: <strong>{playerName}</strong></li>
                    <li>Skill: <strong>{skillKey}</strong></li>
                </ul>
                <p>Please log in to the dashboard to update your notes.</p>
                <br/>
                <p style='color: #888; font-size: 12px;'>— Football Scout Dashboard</p>
            </div>
        </div>";

        await SendEmailAsync(toEmail, scoutName, subject, html);
    }
}