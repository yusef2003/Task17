namespace CinemaDashboard.Models
{
    public class Category : IEntity
    {
        public int Id { get; set; }
        [MinMaxCustom(2, 20)]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool Status { get; set; }
    }
}
