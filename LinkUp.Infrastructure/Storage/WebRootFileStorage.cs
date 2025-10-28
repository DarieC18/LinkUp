using LinkUp.Application.Interfaces.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace LinkUp.Infrastructure.Storage
{
    public sealed class WebRootFileStorage : IFileStorage
    {
        private readonly string _root;

        public WebRootFileStorage(IWebHostEnvironment env)
        {
            var webroot = env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            _root = Path.Combine(webroot, "uploads", "posts");
            Directory.CreateDirectory(_root);
        }

        public async Task<string> SavePostImageAsync(IFormFile file, long maxBytes)
        {
            if (file.Length > maxBytes)
                throw new InvalidOperationException("La imagen excede el tamaño permitido.");

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext))
                throw new InvalidOperationException("Formato de imagen no permitido.");

            var year = DateTime.UtcNow.ToString("yyyy");
            var month = DateTime.UtcNow.ToString("MM");
            var folder = Path.Combine(_root, year, month);
            Directory.CreateDirectory(folder);

            var filename = $"{Guid.NewGuid():N}{ext}";
            var abs = Path.Combine(folder, filename);
            using (var fs = new FileStream(abs, FileMode.Create))
                await file.CopyToAsync(fs);

            var rel = abs[(abs.IndexOf("wwwroot", StringComparison.OrdinalIgnoreCase) + "wwwroot".Length)..]
                .Replace('\\', '/');
            return rel;
        }
    }
}
