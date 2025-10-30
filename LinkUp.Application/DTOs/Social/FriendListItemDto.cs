namespace LinkUp.Application.DTOs.Social
{
    public class FriendListItemDto
    {
        public string UserId { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public string? AvatarUrl { get; set; }
        public int MutualFriendsCount { get; set; }
    }
}
