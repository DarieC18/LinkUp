namespace LinkUp.Application.Interfaces.Users
{
    public sealed class UserBasicDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? AvatarPath { get; set; }
    }

    public interface IUsersReadOnly
    {
        Task<UserBasicDto?> GetBasicAsync(string userId);
    }
}
