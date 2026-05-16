namespace CinemaDashboard
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            // Database
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer("Data Source=.;Initial Catalog=CinemaDashboard;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;"));

            // Identity
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail    = true;
                options.Password.RequireDigit           = true;
                options.Password.RequiredLength         = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase       = false;
                options.Password.RequireLowercase       = false;
                options.Lockout.DefaultLockoutTimeSpan  = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Cookie
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath        = "/Identity/Account/Login";
                options.LogoutPath       = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.ExpireTimeSpan   = TimeSpan.FromDays(30);
                options.SlidingExpiration = true;
            });

            // Session (for cart)
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout        = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly    = true;
                options.Cookie.IsEssential = true;
            });

            // App services
            builder.Services.AddScoped<IImageService,      ImageService>();
            builder.Services.AddScoped<IEmailService,      EmailService>();
            builder.Services.AddScoped<IMovieRepository,   MovieRepository>();
            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IRepository<Actor>,    Repository<Actor>>();
            builder.Services.AddScoped<IRepository<Category>, Repository<Category>>();
            builder.Services.AddScoped<IRepository<Cinema>,   Repository<Cinema>>();

            var app = builder.Build();

            // Seed roles + admin accounts
            using (var scope = app.Services.CreateScope())
                await DbInitializer.SeedAsync(scope.ServiceProvider);

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(name: "areas",   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(name: "default", pattern: "{area=Identity}/{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
