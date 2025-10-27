using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LinkUp.Application.ViewModels.Profile
{
    public class EditProfileViewModel
    {
        [Required, MaxLength(60)]
        public string FirstName { get; set; } = "";

        [Required, MaxLength(60)]
        public string LastName { get; set; } = "";

        [Required, Phone, MaxLength(20)]
        public string PhoneNumber { get; set; } = "";

        public IFormFile? ProfilePhoto { get; set; }

        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        public string? Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        public string? ConfirmPassword { get; set; }
    }
}
