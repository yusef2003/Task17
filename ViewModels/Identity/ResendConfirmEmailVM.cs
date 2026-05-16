namespace CinemaDashboard.ViewModels.Identity
{
    public class ResendConfirmEmailVM
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
