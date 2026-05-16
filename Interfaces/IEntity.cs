namespace CinemaDashboard.Interfaces
{
    // Marker interface so Repository<T> can access .Id without reflection or EF.Property
    public interface IEntity
    {
        int Id { get; set; }
    }
}
