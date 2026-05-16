namespace CinemaDashboard.Services
{
    // SRP: This class has ONE job — manage image files on disk
    // OCP: New image storage strategies (e.g. cloud) can extend IImageService without modifying this
    public class ImageService : IImageService
    {
        public string SaveImage(IFormFile file, string subFolder)
        {
            var fileName = Guid.NewGuid().ToString() + "-" + file.FileName;
            var dir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", subFolder);
            Directory.CreateDirectory(dir);
            using var stream = File.Create(Path.Combine(dir, fileName));
            file.CopyTo(stream);
            return fileName;
        }

        public void DeleteImage(string fileName, string subFolder)
        {
            if (string.IsNullOrEmpty(fileName) || fileName == "default.png") return;
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", subFolder, fileName);
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
