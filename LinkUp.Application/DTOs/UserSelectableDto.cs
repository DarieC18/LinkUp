namespace LinkUp.Application.DTOs
{
    public sealed class UserSelectableDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarPath { get; set; }
        public int MutualFriendsCount { get; set; }
    }
}
