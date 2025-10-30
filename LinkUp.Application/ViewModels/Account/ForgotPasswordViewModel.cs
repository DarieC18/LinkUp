using System.ComponentModel.DataAnnotations;

namespace LinkUp.Application.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public string UserName { get; set; } = "";
    }
}
