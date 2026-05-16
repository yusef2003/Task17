namespace CinemaDashboard.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
            => _userManager = userManager;

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.ToList();
            var result = new List<(ApplicationUser User, IList<string> Roles, bool IsLockedOut)>();
            foreach (var user in users)
            {
                var roles     = await _userManager.GetRolesAsync(user);
                var lockedOut = await _userManager.IsLockedOutAsync(user);
                result.Add((user, roles, lockedOut));
            }
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> Lock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.SetLockoutEnabledAsync(user, true);
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
                TempData["Success-Notification"] = $"{user.FullName} has been locked.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Unlock(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.SetLockoutEndDateAsync(user, null);
                TempData["Success-Notification"] = $"{user.FullName} has been unlocked.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
