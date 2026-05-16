namespace CinemaDashboard.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Area("Admin")]
    public class CategoryController : Controller
    {
        // DIP: Depends on abstraction, not ApplicationDbContext directly
        private readonly IRepository<Category> _categoryRepo;

        public CategoryController(IRepository<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        // SRP: Controller only handles HTTP flow — DB work delegated to repository
        public IActionResult Index() => View(_categoryRepo.GetAll());

        [HttpGet]
        public IActionResult Create() => View(new Category());

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid) { TempData["Error-Notification"] = "Invalid Data"; return View(category); }
            _categoryRepo.Add(category);
            _categoryRepo.Save();
            TempData["Success-Notification"] = "Category Created Successfully";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            var category = _categoryRepo.GetByIdNoTracking(id);
            if (category == null) return RedirectToAction("NotFoundPage", "Home");
            return View(category);
        }

        [HttpPost]
        public IActionResult Update(Category category)
        {
            if (!ModelState.IsValid) return View(category);
            // Update uses SetValues internally — no duplicate-tracking conflict
            _categoryRepo.Update(category);
            _categoryRepo.Save();
            TempData["Success-Notification"] = "Category Updated Successfully";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Delete(int id)
        {
            // GetById is fine here — we use the tracked entity to delete it immediately
            var category = _categoryRepo.GetById(id);
            if (category == null) return RedirectToAction("NotFoundPage", "Home");
            _categoryRepo.Delete(category);
            _categoryRepo.Save();
            TempData["Success-Notification"] = "Category Deleted Successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
