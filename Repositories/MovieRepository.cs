namespace CinemaDashboard.Repositories
{
    // SRP: Handles only Movie-related data access
    // OCP: Extends Repository<Movie> and IMovieRepository — new features added here, not in controllers
    // LSP: Can substitute IMovieRepository or IRepository<Movie> anywhere
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly IImageService _imageService;

        public MovieRepository(ApplicationDbContext context, IImageService imageService) : base(context)
        {
            _imageService = imageService;
        }

        public IQueryable<Movie> GetMoviesWithDetails() =>
            _context.Movies.Include(m => m.Category).Include(m => m.Cinema);

        public IQueryable<Movie> GetActiveMoviesWithDetails() =>
            _context.Movies.Where(m => m.Status == true).Include(m => m.Category).Include(m => m.Cinema);

        public void AddSubImages(int movieId, List<IFormFile> files)
        {
            var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "movieSubImages");
            Directory.CreateDirectory(dir);
            foreach (var file in files)
            {
                if (file == null || file.Length == 0) continue;
                var fileName = _imageService.SaveImage(file, "movieSubImages");
                _context.MovieSubImages.Add(new MovieSubImage { MovieId = movieId, Img = fileName });
            }
        }

        public void DeleteSubImages(int movieId)
        {
            var subImages = _context.MovieSubImages.Where(ms => ms.MovieId == movieId).ToList();
            foreach (var img in subImages) _imageService.DeleteImage(img.Img, "movieSubImages");
            _context.MovieSubImages.RemoveRange(subImages);
        }

        public void UpdateActors(int movieId, List<int>? actorIds)
        {
            var old = _context.MovieActors.Where(ma => ma.MovieId == movieId).ToList();
            _context.MovieActors.RemoveRange(old);
            if (actorIds == null) return;
            foreach (var actorId in actorIds)
                _context.MovieActors.Add(new MovieActor { MovieId = movieId, ActorId = actorId });
        }

        public List<MovieSubImage> GetSubImages(int movieId) =>
            _context.MovieSubImages.Where(ms => ms.MovieId == movieId).ToList();

        public List<int> GetActorIds(int movieId) =>
            _context.MovieActors.Where(ma => ma.MovieId == movieId).Select(ma => ma.ActorId).ToList();
    }
}
