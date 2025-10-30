using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

public class PostCreateVm
{
    [Required, StringLength(1000)]
    public string Content { get; set; } = "";

    [Required]
    public string MediaType { get; set; } = "";

    public IFormFile? ImageFile { get; set; }

    public string? YouTubeUrl { get; set; }
}
