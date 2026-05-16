namespace CinemaDashboard.Interfaces
{
    // ISP: Extends the generic interface with Movie-specific concerns only
    // OCP: New query methods added here — controllers never need to change
    public interface IMovieRepository : IRepository<Movie>
    {
        IQueryable<Movie> GetMoviesWithDetails();
        IQueryable<Movie> GetActiveMoviesWithDetails();
        void AddSubImages(int movieId, List<IFormFile> files);
        void DeleteSubImages(int movieId);
        void UpdateActors(int movieId, List<int>? actorIds);
        List<MovieSubImage> GetSubImages(int movieId);
        List<int> GetActorIds(int movieId);
    }
}
