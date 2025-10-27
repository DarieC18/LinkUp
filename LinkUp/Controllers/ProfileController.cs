using LinkUp.Application.ViewModels.Profile;
using LinkUp.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AccountServiceForWebApp _accounts;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] _allowedAvatarMime = { "image/jpeg", "image/png", "image/webp" };
        private const long _maxAvatarBytes = 2 * 1024 * 1024;

        public ProfileController(AccountServiceForWebApp accounts, IWebHostEnvironment env)
        {
            _accounts = accounts;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
            var dto = await _accounts.GetProfileAsync(userId);
            if (dto is null) return NotFound();

            var vm = new EditProfileViewModel
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber ?? string.Empty
            };
            ViewBag.CurrentPhoto = dto.ProfilePhotoPath;
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(EditProfileViewModel vm, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                var userIdRe = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
                var dtoRe = await _accounts.GetProfileAsync(userIdRe);
                ViewBag.CurrentPhoto = dtoRe?.ProfilePhotoPath;
                return View(vm);
            }

            string? newPhotoVirtual = null;
            if (vm.ProfilePhoto is not null && vm.ProfilePhoto.Length > 0)
            {
                var err = ValidateAvatar(vm.ProfilePhoto);
                if (err is not null)
                {
                    ModelState.AddModelError(nameof(vm.ProfilePhoto), err);
                    var userIdErr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;
                    var dtoErr = await _accounts.GetProfileAsync(userIdErr);
                    ViewBag.CurrentPhoto = dtoErr?.ProfilePhotoPath;
                    return View(vm);
                }
                newPhotoVirtual = await SaveAvatarAsync(vm.ProfilePhoto, ct);
            }

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value;

            var (ok, errors) = await _accounts.UpdateProfileAndMaybePasswordAsync(
                userId,
                vm.FirstName,
                vm.LastName,
                vm.PhoneNumber,
                newPhotoVirtual,
                vm.Password,
                vm.ConfirmPassword
            );

            if (!ok)
            {
                foreach (var e in errors) ModelState.AddModelError(string.Empty, e);
                var dto = await _accounts.GetProfileAsync(userId);
                ViewBag.CurrentPhoto = dto?.ProfilePhotoPath;
                return View(vm);
            }

            TempData["Info"] = "Perfil actualizado correctamente.";
            return RedirectToAction("Index", "Home");
        }

        // Helpers
        private string? ValidateAvatar(IFormFile file)
        {
            if (!_allowedAvatarMime.Contains(file.ContentType)) return "Formato no permitido. Solo JPG, PNG o WebP.";
            if (file.Length > _maxAvatarBytes) return "El archivo excede 2 MB.";
            return null;
        }

        private async Task<string> SaveAvatarAsync(IFormFile file, CancellationToken ct)
        {
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var avatarsDir = Path.Combine(webRoot, "uploads", "avatars");
            Directory.CreateDirectory(avatarsDir);

            var ext = Path.GetExtension(file.FileName);
            var fileName = $"{Guid.NewGuid()}{ext}";
            var physicalPath = Path.Combine(avatarsDir, fileName);

            await using (var fs = System.IO.File.Create(physicalPath))
                await file.CopyToAsync(fs, ct);

            return $"/uploads/avatars/{fileName}";
        }
    }
}
