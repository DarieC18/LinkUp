namespace LinkUp.Application.DTOs.Battleship
{
    public class FriendOptionDto
    {
        public string UserId { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string? AvatarUrl { get; set; }
        public int MutualFriends { get; set; }
    }
}
