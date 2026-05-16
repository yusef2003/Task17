namespace CinemaDashboard.ViewModels
{
    public class MovieVM
    {
        public Movie? Movie { get; set; }
        public List<Category> Categories { get; set; } = new();
        public List<Cinema> Cinemas { get; set; } = new();
        public List<Actor> AllActors { get; set; } = new();
        public List<int> SelectedActorIds { get; set; } = new();
        public List<MovieSubImage>? MovieSubImages { get; set; }
    }
}
