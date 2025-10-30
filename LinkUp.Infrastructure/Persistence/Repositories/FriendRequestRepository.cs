using LinkUp.Application.Interfaces.Social;
using LinkUp.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Persistence.Repositories
{
    public class FriendRequestRepository : IFriendRequestRepository
    {
        private readonly ApplicationDbContext _db;
        public FriendRequestRepository(ApplicationDbContext db) => _db = db;

        public Task<FriendRequest?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => _db.FriendRequests.FirstOrDefaultAsync(r => r.Id == id, ct);

        public async Task<bool> ExistsActivePairAsync(string userAId, string userBId, CancellationToken ct = default)
        {
            return await _db.FriendRequests.AnyAsync(r =>
                r.Status == FriendRequestStatus.Pending &&
                ((r.FromUserId == userAId && r.ToUserId == userBId) ||
                 (r.FromUserId == userBId && r.ToUserId == userAId)), ct);
        }

        public Task<List<FriendRequest>> GetPendingReceivedAsync(string userId, CancellationToken ct = default)
            => _db.FriendRequests
                  .Where(r => r.ToUserId == userId && r.Status == FriendRequestStatus.Pending)
                  .OrderByDescending(r => r.CreatedAtUtc)
                  .ToListAsync(ct);

        public Task<int> CountPendingReceivedAsync(string userId, CancellationToken ct = default)
            => _db.FriendRequests.CountAsync(r => r.ToUserId == userId && r.Status == FriendRequestStatus.Pending, ct);

        public Task<List<FriendRequest>> GetSentAsync(string userId, CancellationToken ct = default)
            => _db.FriendRequests
                  .Where(r => r.FromUserId == userId && r.Status == FriendRequestStatus.Pending)
                  .OrderByDescending(r => r.CreatedAtUtc)
                  .ToListAsync(ct);

        public async Task AddAsync(FriendRequest entity, CancellationToken ct = default)
        {
            _db.FriendRequests.Add(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(FriendRequest entity, CancellationToken ct = default)
        {
            _db.FriendRequests.Update(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(FriendRequest entity, CancellationToken ct = default)
        {
            _db.FriendRequests.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}
