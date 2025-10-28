namespace LinkUp.Application.DTOs.Social
{
    public sealed class PostForEditDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = default!;
        public string? ImagePath { get; set; }
        public string? YouTubeVideoId { get; set; }
        public bool IsMine { get; set; }
    }
}
