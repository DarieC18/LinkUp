namespace LinkUp.Application.DTOs.Social
{
    public sealed class CommentDto
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }
        public string UserId { get; set; } = default!;
        public Guid? ParentCommentId { get; set; }
        public string Content { get; set; } = default!;
        public DateTime CreatedAtUtc { get; set; }
        public string AuthorName { get; set; } = "Usuario";
        public string? AuthorAvatarPath { get; set; }
        public List<CommentDto> Replies { get; set; } = new();
        public bool IsMine { get; set; } // para UI (editar/borrar)

    }
}
