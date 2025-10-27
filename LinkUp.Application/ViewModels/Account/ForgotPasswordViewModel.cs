using System.ComponentModel.DataAnnotations;

namespace LinkUp.Application.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = "";
    }
}
