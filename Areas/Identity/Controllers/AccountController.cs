namespace CinemaDashboard.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser>   _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService                  _emailService;

        public AccountController(
            UserManager<ApplicationUser>   userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailService                  emailService)
        {
            _userManager   = userManager;
            _signInManager = signInManager;
            _emailService  = emailService;
        }

        // ─── REGISTER ─────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                FullName = model.FullName,
                UserName = model.Email,
                Email    = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
                return View(model);
            }

            // Assign Customer role
            await _userManager.AddToRoleAsync(user, "Customer");

            var token      = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmUrl = Url.Action("ConfirmEmail", "Account",
                                new { area = "Identity", userId = user.Id, token },
                                Request.Scheme)!;

            await _emailService.SendEmailAsync(user.Email!,
                "🎬 Confirm Your Cinema Account",
                EmailTemplates.ConfirmEmail(user.FullName, confirmUrl));

            TempData["Info"] = "Registration successful! Please check your email to confirm your account.";
            return RedirectToAction(nameof(Login));
        }

        // ─── LOGIN ────────────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                TempData["Error"] = "Please confirm your email before logging in.";
                return View(model);
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                ModelState.AddModelError("", "Your account is locked. Please contact the administrator.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            TempData["Success-Notification"] = $"Welcome back, {user.FullName}!";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Redirect based on role
            if (await _userManager.IsInRoleAsync(user, "SuperAdmin") ||
                await _userManager.IsInRoleAsync(user, "Admin"))
                return RedirectToAction("Index", "Home", new { area = "Admin" });

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        // ─── LOGOUT ───────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account", new { area = "Identity" });
        }

        // ─── CONFIRM EMAIL ────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return RedirectToAction(nameof(Login));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return RedirectToAction(nameof(Login));

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                TempData["Success-Notification"] = "Email confirmed! You can now log in.";
                return RedirectToAction(nameof(Login));
            }

            TempData["Error"] = "Email confirmation failed. The link may have expired.";
            return RedirectToAction(nameof(Login));
        }

        // ─── RESEND CONFIRMATION EMAIL ────────────────────────────────────────
        [HttpGet]
        public IActionResult ResendEmailConfirmation() => View();

        [HttpPost]
        public async Task<IActionResult> ResendEmailConfirmation(ResendConfirmEmailVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
            {
                var token      = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmUrl = Url.Action("ConfirmEmail", "Account",
                                    new { area = "Identity", userId = user.Id, token },
                                    Request.Scheme)!;
                await _emailService.SendEmailAsync(user.Email!,
                    "🎬 Confirm Your Cinema Account",
                    EmailTemplates.ConfirmEmail(user.FullName, confirmUrl));
            }

            TempData["Info"] = "If that email exists and is unconfirmed, a new confirmation link has been sent.";
            return RedirectToAction(nameof(Login));
        }

        // ─── FORGOT PASSWORD ──────────────────────────────────────────────────
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && await _userManager.IsEmailConfirmedAsync(user))
            {
                var token    = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetUrl = Url.Action("ResetPassword", "Account",
                                  new { area = "Identity", token, email = user.Email },
                                  Request.Scheme)!;
                await _emailService.SendEmailAsync(user.Email!,
                    "🔑 Reset Your Cinema Password",
                    EmailTemplates.ResetPassword(user.FullName, resetUrl));
            }

            TempData["Info"] = "If that email is registered, a password reset link has been sent.";
            return RedirectToAction(nameof(Login));
        }

        // ─── RESET PASSWORD ───────────────────────────────────────────────────
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
            => View(new ResetPasswordVM { Token = token, Email = email });

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["Error"] = "Invalid request.";
                return RedirectToAction(nameof(Login));
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                TempData["Success-Notification"] = "Password reset successful! You can now log in.";
                return RedirectToAction(nameof(Login));
            }

            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }

        // ─── PROFILE ──────────────────────────────────────────────────────────
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            var vm = new EditProfileVM
            {
                FullName    = user.FullName,
                Address     = user.Address,
                PhoneNumber = user.PhoneNumber
            };
            return View(vm);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Profile(EditProfileVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            user.FullName    = model.FullName;
            user.Address     = model.Address;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success-Notification"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }

        // ─── CHANGE PASSWORD ──────────────────────────────────────────────────
        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword() => View();

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success-Notification"] = "Password changed successfully!";
                return RedirectToAction(nameof(Profile));
            }

            foreach (var e in result.Errors) ModelState.AddModelError("", e.Description);
            return View(model);
        }

        // ─── ACCESS DENIED ────────────────────────────────────────────────────
        [HttpGet]
        public IActionResult AccessDenied() => View();
    }
}
