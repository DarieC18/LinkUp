namespace LinkUp.Application.Interfaces.Users
{
    public sealed class UserBasicDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? AvatarPath { get; set; }
    }
    public sealed class UserBasicRowDto
    {
        public string Id { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? AvatarPath { get; set; }
    }
    public interface IUsersReadOnly
    {
        Task<UserBasicDto?> GetBasicAsync(string userId, CancellationToken ct = default);
        Task<List<UserBasicRowDto>> GetAllBasicAsync(CancellationToken ct = default);
        Task<List<UserBasicRowDto>> SearchBasicAsync(string term, CancellationToken ct = default);
        Task<string?> GetIdByUserNameAsync(string username, CancellationToken ct = default);

    }
}
