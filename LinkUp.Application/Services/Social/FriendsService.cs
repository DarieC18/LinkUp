using LinkUp.Application.DTOs;
using LinkUp.Application.DTOs.Social;
using LinkUp.Application.Interfaces.Social;
using LinkUp.Application.Interfaces.Users;
using LinkUp.Domain.Entities.Social;
using static LinkUp.Domain.Entities.Social.FriendRequestStatus;

namespace LinkUp.Application.Services.Social
{
    public class FriendsService : IFriendsService
    {
        private readonly IFriendRequestRepository _requests;
        private readonly IFriendshipRepository _friends;
        private readonly IUsersReadOnly _users;
        private readonly IDateTime _clock;

        public FriendsService(
            IFriendRequestRepository requests,
            IFriendshipRepository friends,
            IUsersReadOnly users,
            IDateTime clock)
        {
            _requests = requests;
            _friends = friends;
            _users = users;
            _clock = clock;
        }

        public async Task CreateRequestAsync(string fromUserId, string toUserId, CancellationToken ct = default)
        {
            if (fromUserId == toUserId) throw new InvalidOperationException("No puedes enviarte una solicitud.");
            if (await _friends.AreFriendsAsync(fromUserId, toUserId, ct))
                throw new InvalidOperationException("Ya son amigos.");
            if (await _requests.ExistsActivePairAsync(fromUserId, toUserId, ct))
                throw new InvalidOperationException("Ya existe una solicitud activa entre ustedes.");

            var req = new FriendRequest
            {
                Id = Guid.NewGuid(),
                FromUserId = fromUserId,
                ToUserId = toUserId,
                Status = Pending,
                CreatedAtUtc = _clock.UtcNow
            };
            await _requests.AddAsync(req, ct);
        }

        public async Task AcceptAsync(Guid requestId, string actingUserId, CancellationToken ct = default)
        {
            var req = await _requests.GetByIdAsync(requestId, ct)
                      ?? throw new KeyNotFoundException("Solicitud no encontrada.");

            if (req.Status != Pending) throw new InvalidOperationException("La solicitud no está pendiente.");
            if (req.ToUserId != actingUserId) throw new UnauthorizedAccessException("No puedes aceptar esta solicitud.");

            var (u1, u2) = Normalize(req.FromUserId, req.ToUserId);
            await _friends.AddAsync(new Friendship
            {
                UserId1 = u1,
                UserId2 = u2,
                CreatedAtUtc = _clock.UtcNow
            }, ct);

            req.Status = Accepted;
            req.RespondedAtUtc = _clock.UtcNow;
            await _requests.UpdateAsync(req, ct);
        }

        public async Task RejectAsync(Guid requestId, string actingUserId, CancellationToken ct = default)
        {
            var req = await _requests.GetByIdAsync(requestId, ct)
                      ?? throw new KeyNotFoundException("Solicitud no encontrada.");

            if (req.Status != Pending) throw new InvalidOperationException("La solicitud no está pendiente.");
            if (req.ToUserId != actingUserId) throw new UnauthorizedAccessException("No puedes rechazar esta solicitud.");

            req.Status = Rejected;
            req.RespondedAtUtc = _clock.UtcNow;
            await _requests.UpdateAsync(req, ct);
        }

        public async Task CancelAsync(Guid requestId, string actingUserId, CancellationToken ct = default)
        {
            var req = await _requests.GetByIdAsync(requestId, ct)
                      ?? throw new KeyNotFoundException("Solicitud no encontrada.");

            if (req.Status != Pending) throw new InvalidOperationException("La solicitud no está pendiente.");
            if (req.FromUserId != actingUserId) throw new UnauthorizedAccessException("No puedes cancelar esta solicitud.");

            await _requests.DeleteAsync(req, ct);
        }
        public async Task<List<FriendRequestListItemDto>> GetPendingReceivedAsync(string userId, CancellationToken ct = default)
        {
            var items = await _requests.GetPendingReceivedAsync(userId, ct);
            var dtos = new List<FriendRequestListItemDto>(items.Count);

            foreach (var r in items)
            {
                var u = await _users.GetBasicAsync(r.FromUserId, ct);
                var mutuals = await _friends.CountMutualFriendsAsync(userId, r.FromUserId, ct);

                dtos.Add(new FriendRequestListItemDto
                {
                    Id = r.Id,
                    FromUserId = r.FromUserId,
                    ToUserId = r.ToUserId,
                    DisplayName = u?.FullName ?? "(usuario)",
                    AvatarUrl = u?.AvatarPath,
                    CreatedAtUtc = r.CreatedAtUtc,
                    MutualFriendsCount = mutuals
                });
            }
            return dtos;
        }
        public async Task<List<FriendRequestListItemDto>> GetSentAsync(string userId, CancellationToken ct = default)
        {
            var items = await _requests.GetSentAsync(userId, ct);
            var dtos = new List<FriendRequestListItemDto>(items.Count);

            foreach (var r in items)
            {
                var u = await _users.GetBasicAsync(r.ToUserId, ct);
                var mutuals = await _friends.CountMutualFriendsAsync(userId, r.ToUserId, ct);

                dtos.Add(new FriendRequestListItemDto
                {
                    Id = r.Id,
                    FromUserId = r.FromUserId,
                    ToUserId = r.ToUserId,
                    DisplayName = u?.FullName ?? "(usuario)",
                    AvatarUrl = u?.AvatarPath,
                    CreatedAtUtc = r.CreatedAtUtc,
                    MutualFriendsCount = mutuals
                });
            }
            return dtos;
        }

        public async Task<List<FriendListItemDto>> ListFriendsAsync(string userId, CancellationToken ct = default)
        {
            var friendIds = await _friends.ListFriendIdsAsync(userId, ct);
            var list = new List<FriendListItemDto>(friendIds.Count);

            foreach (var fid in friendIds)
            {
                var u = await _users.GetBasicAsync(fid, ct);
                var mutuals = await _friends.CountMutualFriendsAsync(userId, fid, ct);

                list.Add(new FriendListItemDto
                {
                    UserId = fid,
                    DisplayName = u?.FullName ?? "(usuario)",
                    AvatarUrl = u?.AvatarPath,
                    MutualFriendsCount = mutuals
                });
            }
            return list;
        }
        public async Task<List<UserSelectableDto>> GetUsersAvailableToRequestAsync(string userId, string? search = null, CancellationToken ct = default)
        {
            var baseList = string.IsNullOrWhiteSpace(search)
                ? await _users.GetAllBasicAsync(ct)
                : await _users.SearchBasicAsync(search!, ct);

            var friends = await _friends.ListFriendIdsAsync(userId, ct);

            var result = new List<UserSelectableDto>(baseList.Count);
            foreach (var u in baseList)
            {
                if (u.Id == userId) continue;
                if (friends.Contains(u.Id)) continue;
                if (await _requests.ExistsActivePairAsync(userId, u.Id, ct)) continue;

                var mutuals = await _friends.CountMutualFriendsAsync(userId, u.Id, ct);
                result.Add(new UserSelectableDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    AvatarPath = u.AvatarPath,
                    MutualFriendsCount = mutuals
                });
            }
            return result;
        }

        public Task RemoveFriendAsync(string userId, string friendId, CancellationToken ct = default)
    => _friends.DeleteAsync(userId, friendId, ct);


        public Task<int> CountPendingAsync(string userId, CancellationToken ct = default)
            => _requests.CountPendingReceivedAsync(userId, ct);

        private static (string u1, string u2) Normalize(string a, string b)
            => string.CompareOrdinal(a, b) < 0 ? (a, b) : (b, a);
    }

    public interface IDateTime { DateTime UtcNow { get; } }
}
