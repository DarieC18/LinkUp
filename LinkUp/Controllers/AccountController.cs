using AutoMapper;
using LinkUp.Application.Auth;
using LinkUp.Application.ViewModels.Account;
using LinkUp.Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LinkUp.Web.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private static readonly string[] _allowedAvatarMime = { "image/jpeg", "image/png", "image/webp" };
        private const long _maxAvatarBytes = 2 * 1024 * 1024;

        private readonly AccountServiceForWebApp _accounts;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public AccountController(AccountServiceForWebApp accounts, IMapper mapper, IWebHostEnvironment env)
        {
            _accounts = accounts;
            _mapper = mapper;
            _env = env;
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
            return View(new RegisterViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);

            if (vm.ProfilePhoto is null || vm.ProfilePhoto.Length == 0)
            {
                ModelState.AddModelError(nameof(vm.ProfilePhoto), "La foto de perfil es obligatoria.");
                return View(vm);
            }

            var err = ValidateAvatar(vm.ProfilePhoto);
            if (err is not null) { ModelState.AddModelError(nameof(vm.ProfilePhoto), err); return View(vm); }

            var avatarVirtualPath = await SaveAvatarAsync(vm.ProfilePhoto, ct);

            var dto = _mapper.Map<RegisterDto>(vm);
            var origin = $"{Request.Scheme}://{Request.Host}";
            var (ok, errors) = await _accounts.RegisterAsync(dto, origin, avatarVirtualPath);

            if (!ok)
            {
                foreach (var e in errors) ModelState.AddModelError(string.Empty, e);
                return View(vm);
            }

            TempData["Info"] = "¡Te enviamos un correo para confirmar tu cuenta!";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token)) return BadRequest();

            var (ok, error) = await _accounts.ConfirmEmailAsync(userId, token);
            if (!ok) return BadRequest(error);

            TempData["Info"] = "¡Email confirmado! Ya puedes iniciar sesión.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (Request.Query["msg"] == "auth")
                TempData["Info"] = "Debe iniciar sesión para acceder a esta sección.";

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm, string? returnUrl = null)
        {
            if (!ModelState.IsValid) return View(vm);

            var dto = _mapper.Map<LoginDto>(vm);
            var signIn = await _accounts.PasswordSignInAsync(dto);

            if (signIn.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Cuenta bloqueada temporalmente por intentos fallidos.");
                return View(vm);
            }
            if (signIn.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Debes confirmar tu email antes de iniciar sesión.");
                return View(vm);
            }
            if (!signIn.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Credenciales inválidas.");
                return View(vm);
            }

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _accounts.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var origin = $"{Request.Scheme}://{Request.Host}";
            var _ = await _accounts.SendForgotAsync(vm, origin);
            TempData["Info"] = "Si el email existe y está confirmado, recibirás instrucciones.";
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
                return BadRequest();

            var vm = new ResetPasswordViewModel { Email = email, Token = WebUtility.UrlDecode(token) };
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            var (ok, errors) = await _accounts.ResetPasswordAsync(vm);
            if (!ok)
            {
                foreach (var e in errors) ModelState.AddModelError("", e);
                return View(vm);
            }
            TempData["Info"] = "Contraseña actualizada. Ya puedes iniciar sesión.";
            return RedirectToAction(nameof(Login));
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

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResendConfirmation(LoginViewModel vm)
        {
            var key = vm.UserNameOrEmail?.Trim();
            if (string.IsNullOrWhiteSpace(key))
            {
                TempData["Error"] = "Ingresa tu usuario o email para reenviar la confirmación.";
                return RedirectToAction(nameof(Login));
            }

            var origin = $"{Request.Scheme}://{Request.Host}";
            var (sent, info) = await _accounts.ResendConfirmationAsync(key, origin);

            TempData[sent ? "Info" : "Error"] = info ?? (sent
                ? "Te reenviamos el correo de confirmación."
                : "No fue posible reenviar el correo en este momento.");

            return RedirectToAction(nameof(Login));
        }
    }
}
