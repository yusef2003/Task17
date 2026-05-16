namespace CinemaDashboard.Areas.Customer.Controllers
{
    [Authorize(Roles = "Customer,Admin,SuperAdmin")]
    [Area("Customer")]
    public class HomeController : Controller
    {
        // DIP: All dependencies injected as abstractions
        private readonly IMovieRepository _movieRepo;
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<Cinema> _cinemaRepo;
        private readonly IBookingRepository _bookingRepo;

        public HomeController(
            IMovieRepository movieRepo,
            IRepository<Category> categoryRepo,
            IRepository<Cinema> cinemaRepo,
            IBookingRepository bookingRepo)
        {
            _movieRepo    = movieRepo;
            _categoryRepo = categoryRepo;
            _cinemaRepo   = cinemaRepo;
            _bookingRepo  = bookingRepo;
        }

        // SRP: Controller only handles HTTP flow; filtering/querying delegated to repository
        public IActionResult Index(FilterCustomerMovieVM filter)
        {
            var movies = _movieRepo.GetActiveMoviesWithDetails();

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

            return View(movies.Skip((filter.Page - 1) * 8).Take(8).ToList());
        }

        [HttpPost]
        public IActionResult Book(Booking booking)
        {
            booking.BookedAt = DateTime.Now;
            _bookingRepo.Add(booking);
            _bookingRepo.Save();
            TempData["Success-Notification"] = "Booking confirmed successfully!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult MyBookings(string? phone, int Page = 1)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                ViewBag.TotalPages  = 0;
                ViewBag.CurrentPage = 1;
                ViewBag.Phone       = phone;
                return View(new List<Booking>());
            }

            var bookings = _bookingRepo.GetByPhone(phone);
            ViewBag.TotalPages  = (int)Math.Ceiling(bookings.Count() / 8.0);
            ViewBag.CurrentPage = Page;
            ViewBag.Phone       = phone;

            return View(bookings.Skip((Page - 1) * 8).Take(8).ToList());
        }

        public IActionResult DeleteBooking(int id, string phone)
        {
            var booking = _bookingRepo.GetById(id);
            if (booking != null)
            {
                _bookingRepo.Delete(booking);
                _bookingRepo.Save();
                TempData["Success-Notification"] = "Booking deleted successfully.";
            }
            return RedirectToAction(nameof(MyBookings), new { phone });
        }
    }
}
