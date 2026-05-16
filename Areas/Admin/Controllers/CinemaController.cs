namespace CinemaDashboard.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Area("Admin")]
    public class CinemaController : Controller
    {
        // DIP: Depends on abstractions injected via constructor, not concrete classes
        private readonly IRepository<Cinema> _cinemaRepo;
        private readonly IImageService _imageService;

        public CinemaController(IRepository<Cinema> cinemaRepo, IImageService imageService)
        {
            _cinemaRepo   = cinemaRepo;
            _imageService = imageService;
        }

        // SRP: Controller only handles HTTP request/response flow
        public IActionResult Index() => View(_cinemaRepo.GetAll());

        [HttpGet]
        public IActionResult Create() => View(new Cinema());

        [HttpPost]
        public IActionResult Create(Cinema item, IFormFile? ImgFile)
        {
            if (ImgFile != null && ImgFile.Length > 0)
                item.Img = _imageService.SaveImage(ImgFile, "cinemaImages");

            _cinemaRepo.Add(item);
            _cinemaRepo.Save();
            TempData["Success-Notification"] = "Cinema Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var item = _cinemaRepo.GetByIdNoTracking(id);
            if (item == null) return RedirectToAction("NotFoundPage", "Home");
            return View(item);
        }

        [HttpPost]
        public IActionResult Update(Cinema item, IFormFile? ImgFile)
        {
            // GetByIdNoTracking: reads old image name WITHOUT attaching a second tracked instance
            var old = _cinemaRepo.GetByIdNoTracking(item.Id);
            if (old == null) return RedirectToAction("NotFoundPage", "Home");

            if (ImgFile != null && ImgFile.Length > 0)
            {
                _imageService.DeleteImage(old.Img, "cinemaImages");
                item.Img = _imageService.SaveImage(ImgFile, "cinemaImages");
            }
            else
            {
                item.Img = old.Img; // keep existing image
            }

            // Update uses SetValues internally — no duplicate-tracking conflict
            _cinemaRepo.Update(item);
            _cinemaRepo.Save();
            TempData["Success-Notification"] = "Cinema Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Delete(int id)
        {
            // GetById is fine here — we use the tracked entity to delete it immediately
            var item = _cinemaRepo.GetById(id);
            if (item == null) return RedirectToAction("NotFoundPage", "Home");
            _imageService.DeleteImage(item.Img, "cinemaImages");
            _cinemaRepo.Delete(item);
            _cinemaRepo.Save();
            TempData["Success-Notification"] = "Cinema Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
