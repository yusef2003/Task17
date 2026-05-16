namespace CinemaDashboard.Interfaces
{
    // ISP: Focused interface for booking operations only
    public interface IBookingRepository : IRepository<Booking>
    {
        IQueryable<Booking> GetByPhone(string phone);
    }
}
