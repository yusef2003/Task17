namespace CinemaDashboard.Interfaces
{
    // ISP: Small focused interface for basic CRUD
    // DIP: Controllers depend on this abstraction, not a concrete DbContext
    // T : IEntity ensures every entity has an int Id — no reflection or EF.Property needed
    public interface IRepository<T> where T : class, IEntity
    {
        IEnumerable<T> GetAll();
        // Attaches entity to the EF tracker. Safe for Delete (used immediately).
        T? GetById(int id);
        // Returns entity WITHOUT attaching to the EF tracker.
        // Always use this in Update [HttpPost] to read old data, so no duplicate-tracking conflict occurs.
        T? GetByIdNoTracking(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Save();
    }
}
