namespace CinemaDashboard.Data
{
    public static class DbInitializer
    {
        public const string SUPER_ADMIN_ROLE = "SuperAdmin";
        public const string ADMIN_ROLE        = "Admin";
        public const string CUSTOMER_ROLE     = "Customer";

        public static async Task SeedAsync(IServiceProvider services)
        {
            var context     = services.GetRequiredService<ApplicationDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // Apply any pending migrations
            await context.Database.MigrateAsync();

            // ── Roles ──────────────────────────────────────────────────────────
            string[] roles = { SUPER_ADMIN_ROLE, ADMIN_ROLE, CUSTOMER_ROLE };
            foreach (var role in roles)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            // ── SuperAdmin ─────────────────────────────────────────────────────
            await EnsureUser(userManager,
                fullName : "Super Admin",
                userName : "superadmin@cinema.com",
                email    : "superadmin@cinema.com",
                password : "SuperAdmin@123",
                role     : SUPER_ADMIN_ROLE);

            // ── Admin ──────────────────────────────────────────────────────────
            await EnsureUser(userManager,
                fullName : "Admin User",
                userName : "admin@cinema.com",
                email    : "admin@cinema.com",
                password : "Admin@123",
                role     : ADMIN_ROLE);
        }

        private static async Task EnsureUser(
            UserManager<ApplicationUser> userManager,
            string fullName, string userName, string email, string password, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    FullName       = fullName,
                    UserName       = userName,
                    Email          = email,
                    EmailConfirmed = true   // pre-confirmed — no email needed
                };
                await userManager.CreateAsync(user, password);
            }
            if (!await userManager.IsInRoleAsync(user, role))
                await userManager.AddToRoleAsync(user, role);
        }
    }
}
