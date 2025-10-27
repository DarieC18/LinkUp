namespace LinkUp.Domain.Entities.Social
{
    public class Post
    {
        public Guid Id { get; set; }

        public string UserId { get; set; } = default!;

        public string? Content { get; set; }

        public string? ImagePath { get; set; }
        public string? YouTubeVideoId { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public bool IsDeleted { get; set; }

        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Reaction> Reactions { get; set; } = new List<Reaction>();
    }
}
