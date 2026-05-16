namespace CinemaDashboard.Models
{
    public class Actor : IEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string Img { get; set; } = "default.png";
        public bool Status { get; set; }
    }
}
