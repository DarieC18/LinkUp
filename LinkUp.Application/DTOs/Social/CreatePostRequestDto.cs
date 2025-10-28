using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LinkUp.Application.DTOs.Social
{
    public sealed class CreatePostRequest
    {
        [Required, MinLength(1)]
        public string Content { get; set; } = string.Empty;

        public string? MediaType { get; set; }

        public IFormFile? ImageFile { get; set; }

        [Url]
        public string? YouTubeUrl { get; set; }
    }
}
