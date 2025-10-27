using LinkUp.Application.Auth;
using LinkUp.Application.DTOs;
using LinkUp.Application.ViewModels.Account;
using LinkUp.Infrastructure.Identity.Entities;
using LinkUp.Infrastructure.Shared.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace LinkUp.Infrastructure.Identity.Services
{
    public class AccountServiceForWebApp
    {
        private readonly UserManager<AppUser> _users;
        private readonly SignInManager<AppUser> _signIn;
        private readonly IEmailSender _email;

        public AccountServiceForWebApp(
            UserManager<AppUser> users,
            SignInManager<AppUser> signIn,
            IEmailSender email)
        {
            _users = users;
            _signIn = signIn;
            _email = email;
        }

        public async Task<(bool ok, IEnumerable<string> errors)> RegisterAsync(
            RegisterDto dto, string origin, string? profilePhotoVirtualPath)
        {
            if (await _users.FindByNameAsync(dto.UserName) != null)
                return (false, new[] { "El nombre de usuario ya existe." });

            if (await _users.FindByEmailAsync(dto.Email) != null)
                return (false, new[] { "Ya existe un usuario con este correo." });

            var user = new AppUser
            {
                UserName = dto.UserName,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                EmailConfirmed = false,
                ProfilePhotoPath = profilePhotoVirtualPath
            };

            var res = await _users.CreateAsync(user, dto.Password);
            if (!res.Succeeded) return (false, res.Errors.Select(e => e.Description));

            var confirmUrl = await BuildConfirmEmailUrl(user, origin);
            await _email.SendAsync(
                user.Email!,
                "Confirma tu cuenta – LinkUp",
                $"<p>Hola {System.Net.WebUtility.HtmlEncode(user.FirstName)},</p>" +
                $"<p>Confirma tu cuenta aquí: <a href=\"{confirmUrl}\">Confirmar</a></p>");

            return (true, Array.Empty<string>());
        }

        public async Task<(bool ok, string? error)> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _users.FindByIdAsync(userId);
            if (user is null) return (false, "Usuario no encontrado.");

            var decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var res = await _users.ConfirmEmailAsync(user, decoded);
            return res.Succeeded ? (true, null) : (false, "Token inválido o expirado.");
        }

        public Task<SignInResult> PasswordSignInAsync(LoginDto dto)
            => _signIn.PasswordSignInAsync(dto.UserNameOrEmail.Trim(), dto.Password, dto.RememberMe, lockoutOnFailure: true);

        public Task SignOutAsync() => _signIn.SignOutAsync();

        public async Task<bool> SendForgotAsync(ForgotPasswordViewModel vm, string origin)
        {
            var user = await _users.Users.FirstOrDefaultAsync(u => u.Email == vm.Email);
            if (user is null || !(await _users.IsEmailConfirmedAsync(user))) return false;

            var resetUrl = await BuildResetPasswordUrl(user, origin);
            await _email.SendAsync(
                vm.Email!,
                "Restablecer contraseña – LinkUp",
                $"<p>Hola {System.Net.WebUtility.HtmlEncode(user.FirstName)},</p>" +
                $"<p>Puedes restablecer tu contraseña aquí: <a href=\"{resetUrl}\">Restablecer</a></p>");

            return true;
        }

        public async Task<(bool ok, IEnumerable<string> errors)> ResetPasswordAsync(ResetPasswordViewModel vm)
        {
            var user = await _users.FindByEmailAsync(vm.Email);
            if (user is null) return (true, Array.Empty<string>());

            var decoded = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(vm.Token));
            var res = await _users.ResetPasswordAsync(user, decoded, vm.Password);

            return res.Succeeded ? (true, Array.Empty<string>())
                                 : (false, res.Errors.Select(e => e.Description));
        }

        public async Task<(bool sent, string? info)> ResendConfirmationAsync(string userNameOrEmail, string origin)
        {
            var key = userNameOrEmail.Trim();
            var user = key.Contains('@')
                ? await _users.FindByEmailAsync(key)
                : await _users.FindByNameAsync(key);

            if (user is null) return (true, "Si el usuario existe, te enviaremos un correo de confirmación.");
            if (await _users.IsEmailConfirmedAsync(user)) return (false, "Tu cuenta ya está confirmada.");

            var url = await BuildConfirmEmailUrl(user, origin);
            await _email.SendAsync(
                user.Email!,
                "Confirma tu cuenta – LinkUp",
                $"<p>Hola {System.Net.WebUtility.HtmlEncode(user.FirstName)},</p>" +
                $"<p>Confirma tu cuenta aquí: <a href=\"{url}\">Confirmar</a></p>");

            return (true, "Te reenviamos el correo de confirmación.");
        }

        // PERFIL: obtener datos
        public async Task<ProfileDto?> GetProfileAsync(string userId)
        {
            var u = await _users.Users.AsNoTracking()
                .Where(x => x.Id == userId)
                .Select(x => new ProfileDto
                {
                    Id = x.Id,
                    UserName = x.UserName!,
                    Email = x.Email!,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber,
                    ProfilePhotoPath = x.ProfilePhotoPath
                })
                .FirstOrDefaultAsync();

            return u;
        }

        public async Task<(bool ok, IEnumerable<string> errors)> UpdateProfileAndMaybePasswordAsync(
            string userId,
            string firstName,
            string lastName,
            string phoneNumber,
            string? newPhotoVirtualPath,
            string? newPassword,
            string? confirmPassword)
        {
            var user = await _users.FindByIdAsync(userId);
            if (user is null) return (false, new[] { "Usuario no encontrado." });

            if (!string.IsNullOrWhiteSpace(newPassword))
            {
                if (newPassword != confirmPassword)
                    return (false, new[] { "Las contraseñas no coinciden." });

                var hasPass = await _users.HasPasswordAsync(user);
                if (hasPass)
                {
                    var remove = await _users.RemovePasswordAsync(user);
                    if (!remove.Succeeded) return (false, remove.Errors.Select(e => e.Description));
                }

                var add = await _users.AddPasswordAsync(user, newPassword);
                if (!add.Succeeded) return (false, add.Errors.Select(e => e.Description));
            }

            user.FirstName = firstName;
            user.LastName = lastName;
            user.PhoneNumber = phoneNumber;
            if (!string.IsNullOrWhiteSpace(newPhotoVirtualPath))
                user.ProfilePhotoPath = newPhotoVirtualPath;

            var res = await _users.UpdateAsync(user);
            return res.Succeeded ? (true, Array.Empty<string>()) : (false, res.Errors.Select(e => e.Description));
        }

        private async Task<string> BuildConfirmEmailUrl(AppUser user, string origin)
        {
            var token = await _users.GenerateEmailConfirmationTokenAsync(user);
            var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString($"{origin}/Account/ConfirmEmail", "userId", user.Id);
            url = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(url, "token", encoded);
            return url;
        }

        private async Task<string> BuildResetPasswordUrl(AppUser user, string origin)
        {
            var token = await _users.GeneratePasswordResetTokenAsync(user);
            var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString($"{origin}/Account/ResetPassword", "email", user.Email!);
            url = Microsoft.AspNetCore.WebUtilities.QueryHelpers.AddQueryString(url, "token", encoded);
            return url;
        }
    }
}
