namespace LinkUp.Application.DTOs.Social
{
    public class FriendRequestListItemDto
    {
        public Guid Id { get; set; }
        public string FromUserId { get; set; } = default!;
        public string ToUserId { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string? AvatarUrl { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public int MutualFriendsCount { get; set; }
    }
}
