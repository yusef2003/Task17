namespace CinemaDashboard.Models
{
    public class Movie : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Explicit precision: 10 digits total, 2 decimal places (e.g. 99999999.99)
        // Prevents the EF decimal truncation warning
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public bool Status { get; set; }
        public DateTime DateTime { get; set; }
        public string MainImg { get; set; } = "default.png";
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public int CinemaId { get; set; }
        public Cinema Cinema { get; set; } = null!;
        public List<MovieActor> MovieActors { get; set; } = new();
        public List<MovieSubImage> SubImages { get; set; } = new();
    }
}
