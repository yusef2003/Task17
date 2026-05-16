using System.Text.Json;

namespace CinemaDashboard.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = "Customer,Admin,SuperAdmin")]
    public class CartController : Controller
    {
        private readonly IMovieRepository _movieRepo;
        private const string CartKey = "Cart";

        public CartController(IMovieRepository movieRepo)
            => _movieRepo = movieRepo;

        private List<CartItem> GetCart()
        {
            var json = HttpContext.Session.GetString(CartKey);
            return json == null ? new List<CartItem>()
                                : JsonSerializer.Deserialize<List<CartItem>>(json)!;
        }

        private void SaveCart(List<CartItem> cart)
            => HttpContext.Session.SetString(CartKey, JsonSerializer.Serialize(cart));

        public IActionResult Index()
        {
            var cart = GetCart();
            ViewBag.Total = cart.Sum(c => c.Price * c.Quantity);
            return View(cart);
        }

        [HttpPost]
        public IActionResult AddToCart(int movieId, int quantity = 1)
        {
            var movie = _movieRepo.GetById(movieId);
            if (movie == null) return NotFound();

            var cart = GetCart();
            var existing = cart.FirstOrDefault(c => c.MovieId == movieId);
            if (existing != null)
                existing.Quantity += quantity;
            else
                cart.Add(new CartItem
                {
                    MovieId   = movie.Id,
                    MovieName = movie.Name,
                    Price     = movie.Price,
                    Quantity  = quantity,
                    MainImg   = movie.MainImg
                });

            SaveCart(cart);
            TempData["Success-Notification"] = $"'{movie.Name}' added to cart!";
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int movieId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.MovieId == movieId);
            if (item != null)
            {
                if (quantity <= 0)
                    cart.Remove(item);
                else
                    item.Quantity = quantity;
            }
            SaveCart(cart);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Remove(int movieId)
        {
            var cart = GetCart();
            cart.RemoveAll(c => c.MovieId == movieId);
            SaveCart(cart);
            TempData["Success-Notification"] = "Item removed from cart.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Clear()
        {
            SaveCart(new List<CartItem>());
            TempData["Success-Notification"] = "Cart cleared.";
            return RedirectToAction(nameof(Index));
        }
    }
}
