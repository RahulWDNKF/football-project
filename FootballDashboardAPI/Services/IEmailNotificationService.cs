namespace FootballDashboardAPI.Services
{
    public interface IEmailNotificationService
    {
        Task SendEmailAsync(string toEmail, string toName, string subject, string htmlContent);
        Task SendContractExpiryAlertAsync(string toEmail, string playerName, string contractEndDate, string scoutName);
        Task SendTaskDueAlertAsync(string toEmail, string taskTitle, string dueDate, string assignedTo);
        Task SendReviewFollowUpAsync(string toEmail, string playerName, string skillKey, string followUpDate, string scoutName);
    }
}
