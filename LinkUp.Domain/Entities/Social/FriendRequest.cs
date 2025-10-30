namespace LinkUp.Domain.Entities.Social
{
    public class FriendRequest
    {
        public Guid Id { get; set; }

        public string FromUserId { get; set; } = default!;
        public string ToUserId { get; set; } = default!;

        public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;

        public DateTime CreatedAtUtc { get; set; }
        public DateTime? RespondedAtUtc { get; set; }
    }
}
