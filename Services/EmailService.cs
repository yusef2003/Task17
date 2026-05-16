namespace CinemaDashboard.Services
{
    // SRP: Only responsible for sending emails
    // DIP: Controllers depend on IEmailService, not this class directly
    public class EmailService : IEmailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl             = true,
                UseDefaultCredentials = false,
                Credentials           = new NetworkCredential("youssefashraf2335@gmail.com", "eftfovndmsidbblp")
            };

            var msg = new MailMessage(
                from: "youssefashraf2335@gmail.com",
                to:   toEmail,
                subject,
                htmlBody)
            {
                IsBodyHtml = true
            };

            return client.SendMailAsync(msg);
        }
    }
}
