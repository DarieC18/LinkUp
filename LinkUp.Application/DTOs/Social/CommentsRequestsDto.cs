using System.ComponentModel.DataAnnotations;

namespace LinkUp.Application.DTOs.Social
{
    public sealed class CreateCommentRequest
    {
        [Required] public Guid PostId { get; set; }
        [Required, MinLength(1)] public string Content { get; set; } = default!;
        [Required] public string UserId { get; set; } = default!;
    }

    public sealed class CreateReplyRequest
    {
        [Required] public Guid ParentCommentId { get; set; }
        [Required] public Guid PostId { get; set; }
        [Required, MinLength(1)] public string Content { get; set; } = default!;
        [Required] public string UserId { get; set; } = default!;
    }

    public sealed class EditCommentRequest
    {
        [Required] public Guid CommentId { get; set; }
        [Required] public string UserId { get; set; } = default!;
        [Required, MinLength(1)] public string Content { get; set; } = default!;
    }

    public sealed class DeleteCommentRequest
    {
        [Required] public Guid CommentId { get; set; }
        [Required] public string UserId { get; set; } = default!;
    }
}
