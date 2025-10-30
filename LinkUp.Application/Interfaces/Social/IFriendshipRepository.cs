using LinkUp.Domain.Entities.Social;

namespace LinkUp.Application.Interfaces.Social
{
    public interface IFriendshipRepository
    {
        Task<bool> AreFriendsAsync(string userAId, string userBId, CancellationToken ct = default);
        Task AddAsync(Friendship entity, CancellationToken ct = default);
        Task DeleteAsync(string userAId, string userBId, CancellationToken ct = default);

        Task<List<string>> ListFriendIdsAsync(string userId, CancellationToken ct = default);

        Task<int> CountMutualFriendsAsync(string userAId, string userBId, CancellationToken ct = default);
    }
}
