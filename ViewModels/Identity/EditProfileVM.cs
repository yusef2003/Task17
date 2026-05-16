namespace CinemaDashboard.ViewModels.Identity
{
    public class EditProfileVM
    {
        [Required, Display(Name = "Full Name")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Address")]
        public string? Address { get; set; }

        [Phone, Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }
    }
}
