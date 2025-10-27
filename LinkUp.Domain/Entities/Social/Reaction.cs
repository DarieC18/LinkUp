using LinkUp.Domain.Enums;

namespace LinkUp.Domain.Entities.Social
{
    public class Reaction
    {
        public Guid Id { get; set; }

        public Guid PostId { get; set; }
        public string UserId { get; set; } = default!;

        public ReactionType Type { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public Post Post { get; set; } = default!;
    }
}
