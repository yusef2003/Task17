namespace CinemaDashboard.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Area("Admin")]
    public class MovieController : Controller
    {
        private readonly IMovieRepository _movieRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Cinema> _cinemaRepo;
        private readonly IRepository<Actor> _actorRepo;
        private readonly IImageService _imageService;

        // The subfolder name used for movie main images — one place to change if ever needed
        private const string MovieImageFolder = "movieImages";

        public MovieController(
            IMovieRepository movieRepo,
            IRepository<Category> categoryRepo,
            IRepository<Cinema> cinemaRepo,
            IRepository<Actor> actorRepo,
            IImageService imageService)
        {
            _movieRepo    = movieRepo;
            _categoryRepo = categoryRepo;
            _cinemaRepo   = cinemaRepo;
            _actorRepo    = actorRepo;
            _imageService = imageService;
        }

        public IActionResult Index(FilterMovieVM filter)
        {
            var movies = _movieRepo.GetMoviesWithDetails();

            if (filter.MovieName != null)  { movies = movies.Where(m => m.Name.Contains(filter.MovieName)); ViewBag.MovieName  = filter.MovieName; }
            if (filter.MinPrice > 0)       { movies = movies.Where(m => m.Price >= filter.MinPrice);         ViewBag.MinPrice   = filter.MinPrice; }
            if (filter.MaxPrice > 0)       { movies = movies.Where(m => m.Price <= filter.MaxPrice);         ViewBag.MaxPrice   = filter.MaxPrice; }
            if (filter.CategoryId > 0)     { movies = movies.Where(m => m.CategoryId == filter.CategoryId); ViewBag.CategoryId = filter.CategoryId; }
            if (filter.CinemaId > 0)       { movies = movies.Where(m => m.CinemaId == filter.CinemaId);     ViewBag.CinemaId   = filter.CinemaId; }
            if (filter.IsUpcoming)         { movies = movies.Where(m => m.DateTime > DateTime.Now);          ViewBag.IsUpcoming = true; }

            ViewBag.Categories  = _categoryRepo.GetAll().ToList();
            ViewBag.Cinemas     = _cinemaRepo.GetAll().ToList();
            ViewBag.TotalPages  = (int)Math.Ceiling(movies.Count() / 8.0);
            ViewBag.CurrentPage = filter.Page;

            return View(movies.Skip((filter.Page - 1) * 8).Take(8).AsEnumerable());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new MovieVM
            {
                Movie      = new Movie { DateTime = DateTime.Now },
                Categories = _categoryRepo.GetAll().ToList(),
                Cinemas    = _cinemaRepo.GetAll().ToList(),
                AllActors  = _actorRepo.GetAll().ToList()
            });
        }

        [HttpPost]
        public IActionResult Create(Movie movie, IFormFile? ImgFile, List<IFormFile>? SubImgFiles, List<int>? ActorIds)
        {
            if (ImgFile != null && ImgFile.Length > 0)
                movie.MainImg = _imageService.SaveImage(ImgFile, MovieImageFolder);

            _movieRepo.Add(movie);
            _movieRepo.Save();

            if (SubImgFiles != null && SubImgFiles.Count > 0)
                _movieRepo.AddSubImages(movie.Id, SubImgFiles);

            _movieRepo.UpdateActors(movie.Id, ActorIds);
            _movieRepo.Save();

            TempData["Success-Notification"] = "Movie Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var movie = _movieRepo.GetByIdNoTracking(id);
            if (movie == null) return RedirectToAction("NotFoundPage", "Home");

            return View(new MovieVM
            {
                Movie            = movie,
                Categories       = _categoryRepo.GetAll().ToList(),
                Cinemas          = _cinemaRepo.GetAll().ToList(),
                AllActors        = _actorRepo.GetAll().ToList(),
                SelectedActorIds = _movieRepo.GetActorIds(id),
                MovieSubImages   = _movieRepo.GetSubImages(id)
            });
        }

        [HttpPost]
        public IActionResult Update(Movie movie, IFormFile? ImgFile, List<IFormFile>? SubImgFiles, List<int>? ActorIds)
        {
            var oldMovie = _movieRepo.GetByIdNoTracking(movie.Id);
            if (oldMovie == null) return RedirectToAction("NotFoundPage", "Home");

            if (ImgFile != null && ImgFile.Length > 0)
            {
                _imageService.DeleteImage(oldMovie.MainImg, MovieImageFolder);
                movie.MainImg = _imageService.SaveImage(ImgFile, MovieImageFolder);
            }
            else
            {
                movie.MainImg = oldMovie.MainImg;
            }

            _movieRepo.Update(movie);

            if (SubImgFiles != null && SubImgFiles.Count > 0)
            {
                _movieRepo.DeleteSubImages(movie.Id);
                _movieRepo.AddSubImages(movie.Id, SubImgFiles);
            }

            _movieRepo.UpdateActors(movie.Id, ActorIds);
            _movieRepo.Save();

            TempData["Success-Notification"] = "Movie Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Delete(int id)
        {
            var movie = _movieRepo.GetById(id);
            if (movie == null) return RedirectToAction("NotFoundPage", "Home");

            _imageService.DeleteImage(movie.MainImg, MovieImageFolder);
            _movieRepo.DeleteSubImages(id);
            _movieRepo.Delete(movie);
            _movieRepo.Save();

            TempData["Success-Notification"] = "Movie Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
