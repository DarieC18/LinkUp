using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LinkUp.Application.DTOs.Social
{
    public sealed class EditPostRequest
    {
        [Required] public Guid PostId { get; set; }
        [Required] public string UserId { get; set; } = default!;
        [Required, MinLength(1)] public string Content { get; set; } = default!;
        public IFormFile? Image { get; set; }
        public string? YouTubeUrl { get; set; }
        public long MaxImageBytes { get; set; } = 3 * 1024 * 1024;
    }

    public sealed class DeletePostRequest
    {
        [Required] public Guid PostId { get; set; }
        [Required] public string UserId { get; set; } = default!;
    }
}
