using LinkUp.Application.DTOs;
using LinkUp.Application.DTOs.Social;

namespace LinkUp.Application.Interfaces.Social
{
    public interface IFriendsService
    {
        Task CreateRequestAsync(string fromUserId, string toUserId, CancellationToken ct = default);
        Task AcceptAsync(Guid requestId, string actingUserId, CancellationToken ct = default);
        Task RejectAsync(Guid requestId, string actingUserId, CancellationToken ct = default);
        Task CancelAsync(Guid requestId, string actingUserId, CancellationToken ct = default);

        Task<List<FriendRequestListItemDto>> GetPendingReceivedAsync(string userId, CancellationToken ct = default);
        Task<List<FriendRequestListItemDto>> GetSentAsync(string userId, CancellationToken ct = default);

        Task<List<FriendListItemDto>> ListFriendsAsync(string userId, CancellationToken ct = default);
        Task<int> CountPendingAsync(string userId, CancellationToken ct = default);
        Task RemoveFriendAsync(string userId, string friendId, CancellationToken ct = default);
        Task<List<UserSelectableDto>> GetUsersAvailableToRequestAsync(string userId, string? search = null, CancellationToken ct = default);

    }
}
