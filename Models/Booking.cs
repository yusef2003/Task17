namespace CinemaDashboard.Models
{
    public class Booking : IEntity
    {
        public int Id { get; set; }
        public string? ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public int MovieId { get; set; }
        public Movie Movie { get; set; } = null!;
        public int Seats { get; set; }
        public DateTime BookedAt { get; set; } = DateTime.Now;
    }
}
