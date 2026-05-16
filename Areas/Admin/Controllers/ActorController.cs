namespace CinemaDashboard.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Area("Admin")]
    public class ActorController : Controller
    {
        // DIP: Depends on abstractions injected via constructor, not concrete classes
        private readonly IRepository<Actor> _actorRepo;
        private readonly IImageService _imageService;

        public ActorController(IRepository<Actor> actorRepo, IImageService imageService)
        {
            _actorRepo    = actorRepo;
            _imageService = imageService;
        }

        // SRP: Controller only handles HTTP request/response flow
        public IActionResult Index() => View(_actorRepo.GetAll());

        [HttpGet]
        public IActionResult Create() => View(new Actor());

        [HttpPost]
        public IActionResult Create(Actor item, IFormFile? ImgFile)
        {
            if (ImgFile != null && ImgFile.Length > 0)
                item.Img = _imageService.SaveImage(ImgFile, "actorImages");

            _actorRepo.Add(item);
            _actorRepo.Save();
            TempData["Success-Notification"] = "Actor Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var item = _actorRepo.GetByIdNoTracking(id);
            if (item == null) return RedirectToAction("NotFoundPage", "Home");
            return View(item);
        }

        [HttpPost]
        public IActionResult Update(Actor item, IFormFile? ImgFile)
        {
            // GetByIdNoTracking: reads old image name WITHOUT attaching a second tracked instance
            var old = _actorRepo.GetByIdNoTracking(item.Id);
            if (old == null) return RedirectToAction("NotFoundPage", "Home");

            if (ImgFile != null && ImgFile.Length > 0)
            {
                _imageService.DeleteImage(old.Img, "actorImages");
                item.Img = _imageService.SaveImage(ImgFile, "actorImages");
            }
            else
            {
                item.Img = old.Img; // keep existing image
            }

            // Update uses SetValues internally — no duplicate-tracking conflict
            _actorRepo.Update(item);
            _actorRepo.Save();
            TempData["Success-Notification"] = "Actor Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Delete(int id)
        {
            // GetById is fine here — we use the tracked entity to delete it immediately
            var item = _actorRepo.GetById(id);
            if (item == null) return RedirectToAction("NotFoundPage", "Home");
            _imageService.DeleteImage(item.Img, "actorImages");
            _actorRepo.Delete(item);
            _actorRepo.Save();
            TempData["Success-Notification"] = "Actor Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
