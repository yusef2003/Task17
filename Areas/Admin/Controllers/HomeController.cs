namespace CinemaDashboard.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Area("Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult NotFoundPage() => View();
    }
}
