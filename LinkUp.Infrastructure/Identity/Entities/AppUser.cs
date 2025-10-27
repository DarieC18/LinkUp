using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace LinkUp.Infrastructure.Identity.Entities
{
    public class AppUser : IdentityUser
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = default!;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = default!;
        public string? ProfilePhotoPath { get; set; }
    }
}
