using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LinkUp.Application.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required, StringLength(60)] public string FirstName { get; set; } = "";
        [Required, StringLength(60)] public string LastName { get; set; } = "";

        [Required, EmailAddress] public string Email { get; set; } = "";

        [Required, StringLength(30)] public string UserName { get; set; } = "";

        //telf RD: +1 opcional y prefijos 809/829/849, con o sin guiones
        [Required]
        [RegularExpression(@"^(\+1[- ]?)?(809|829|849)[- ]?\d{3}[- ]?\d{4}$",
            ErrorMessage = "Formato de teléfono inválido. Ej.: 809-555-1234")]
        public string PhoneNumber { get; set; } = "";

        [Required, DataType(DataType.Password), StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = "";

        [Required, DataType(DataType.Password),
         Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmPassword { get; set; } = "";

        [Required(ErrorMessage = "La foto de perfil es obligatoria.")]
        public IFormFile? ProfilePhoto { get; set; }
    }
}
