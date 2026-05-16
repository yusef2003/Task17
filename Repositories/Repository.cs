namespace CinemaDashboard.Repositories
{
    // SRP: Handles ONLY generic data-access for a given entity type
    // DIP: Receives ApplicationDbContext via constructor injection — never newed up
    // LSP: All concrete repositories safely substitute IRepository<T>
    // T : IEntity gives direct access to .Id — no EF.Property, no reflection
    public class Repository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet   = context.Set<T>();
        }

        public IEnumerable<T> GetAll() => _dbSet.AsEnumerable();

        // Attaches the returned entity to the tracker — safe for Delete only
        public T? GetById(int id) => _dbSet.Find(id);

        // Does NOT attach to the tracker — always use this before Update to avoid
        // "another instance with the same key is already being tracked" error
        public T? GetByIdNoTracking(int id) =>
            _dbSet.AsNoTracking().FirstOrDefault(e => e.Id == id);

        public void Add(T entity) => _dbSet.Add(entity);

        // Finds the already-tracked instance by Id and copies new values onto it.
        // This prevents a second object with the same key entering the tracker.
        public void Update(T entity)
        {
            var tracked = _dbSet.Find(entity.Id);
            if (tracked != null)
                _context.Entry(tracked).CurrentValues.SetValues(entity);
            else
                _dbSet.Update(entity);   // fallback: entity was never tracked
        }

        public void Delete(T entity) => _dbSet.Remove(entity);
        public void Save()           => _context.SaveChanges();
    }
}
