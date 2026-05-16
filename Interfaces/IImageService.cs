namespace CinemaDashboard.Interfaces
{
    // ISP: Focused interface for image operations only
    // DIP: Controllers depend on this abstraction, not a concrete class
    public interface IImageService
    {
        string SaveImage(IFormFile file, string subFolder);
        void DeleteImage(string fileName, string subFolder);
    }
}
