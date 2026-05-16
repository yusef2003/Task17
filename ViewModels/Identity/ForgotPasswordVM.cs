namespace CinemaDashboard.ViewModels.Identity
{
    public class ForgotPasswordVM
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
