using LinkUp.Domain.Entities.Social;

namespace LinkUp.Application.Interfaces.Social
{
    public interface IFriendRequestRepository
    {
        Task<FriendRequest?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<bool> ExistsActivePairAsync(string userAId, string userBId, CancellationToken ct = default);
        Task<List<FriendRequest>> GetPendingReceivedAsync(string userId, CancellationToken ct = default);
        Task<int> CountPendingReceivedAsync(string userId, CancellationToken ct = default);
        Task<List<FriendRequest>> GetSentAsync(string userId, CancellationToken ct = default);

        Task AddAsync(FriendRequest entity, CancellationToken ct = default);
        Task UpdateAsync(FriendRequest entity, CancellationToken ct = default);
        Task DeleteAsync(FriendRequest entity, CancellationToken ct = default);
    }
}
