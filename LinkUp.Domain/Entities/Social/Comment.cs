namespace LinkUp.Domain.Entities.Social
{
    public class Comment
    {
        public Guid Id { get; set; }

        public Guid PostId { get; set; }
        public string UserId { get; set; } = default!;

        public Guid? ParentCommentId { get; set; }

        public string Content { get; set; } = default!;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }

        public Post Post { get; set; } = default!;
        public Comment? ParentComment { get; set; }
    }
}
