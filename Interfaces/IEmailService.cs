namespace CinemaDashboard.Interfaces
{
    // ISP: Focused interface for email sending only
    // DIP: AccountController depends on this abstraction
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
    }
}
