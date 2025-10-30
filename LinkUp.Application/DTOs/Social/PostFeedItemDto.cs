namespace LinkUp.Application.DTOs.Social
{
    public class PostFeedItemDto
    {
        public Guid Id { get; set; }
        public string AuthorId { get; set; } = default!;
        public string AuthorName { get; set; } = default!;
        public string? AuthorAvatarPath { get; set; }

        public string? Content { get; set; }
        public string? ImagePath { get; set; }
        public string? YouTubeVideoId { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }

        public bool? MyReactionIsLike { get; set; }
        public bool? MyReactionIsDislike { get; set; }
        public IReadOnlyList<CommentDto>? Comments { get; set; }

    }

    public class GetFeedRequest
    {
        public string CurrentUserId { get; set; } = default!;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? UserIdFilter { get; set; }
    }

    public class PagedResult<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
        public T[] Items { get; set; } = [];
    }
}
