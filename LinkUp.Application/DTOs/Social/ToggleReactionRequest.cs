using LinkUp.Domain.Enums;

namespace LinkUp.Application.DTOs.Social
{
    public class ToggleReactionRequest
    {
        public Guid PostId { get; set; }
        public string UserId { get; set; } = default!;
        public ReactionType ReactionType { get; set; } // Like o Dislike
    }

    public class ToggleReactionResult
    {
        public int LikeCount { get; set; }
        public int DislikeCount { get; set; }
        public string State { get; set; } = "none";
    }
}
