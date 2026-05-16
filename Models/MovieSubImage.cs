using Microsoft.EntityFrameworkCore;
namespace CinemaDashboard.Models { [PrimaryKey(nameof(MovieId), nameof(Img))] public class MovieSubImage { public int MovieId { get; set; } public Movie Movie { get; set; } = null!; 
        public string Img { get; set; } = string.Empty; } }