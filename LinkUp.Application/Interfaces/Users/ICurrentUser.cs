namespace LinkUp.Application.Interfaces.Users
{
    public interface ICurrentUser
    {
        bool IsAuthenticated { get; }
        string? UserId { get; }
        string? UserName { get; }
        string? Email { get; }
    }
}
