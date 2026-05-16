namespace CinemaDashboard.Repositories
{
    // SRP: Handles only Booking-related data access
    // LSP: Safely substitutable for IBookingRepository and IRepository<Booking>
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext context) : base(context) { }

        public IQueryable<Booking> GetByPhone(string phone) =>
            _context.Bookings
                .Where(b => b.CustomerPhone == phone)
                .Include(b => b.Movie)
                .ThenInclude(m => m.Cinema);
    }
}
