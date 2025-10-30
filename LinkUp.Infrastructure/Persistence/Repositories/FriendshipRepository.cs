using LinkUp.Application.Interfaces.Social;
using LinkUp.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Persistence.Repositories
{
    public class FriendshipRepository : IFriendshipRepository
    {
        private readonly ApplicationDbContext _db;
        public FriendshipRepository(ApplicationDbContext db) => _db = db;

        public Task<bool> AreFriendsAsync(string userAId, string userBId, CancellationToken ct = default)
        {
            var (u1, u2) = Normalize(userAId, userBId);
            return _db.Friendships.AnyAsync(f => f.UserId1 == u1 && f.UserId2 == u2, ct);
        }

        public async Task AddAsync(Friendship entity, CancellationToken ct = default)
        {
            _db.Friendships.Add(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(string userAId, string userBId, CancellationToken ct = default)
        {
            var (u1, u2) = Normalize(userAId, userBId);
            var f = await _db.Friendships.FirstOrDefaultAsync(x => x.UserId1 == u1 && x.UserId2 == u2, ct);
            if (f != null)
            {
                _db.Friendships.Remove(f);
                await _db.SaveChangesAsync(ct);
            }
        }

        public async Task<List<string>> ListFriendIdsAsync(string userId, CancellationToken ct = default)
        {
            var left = _db.Friendships.Where(f => f.UserId1 == userId).Select(f => f.UserId2);
            var right = _db.Friendships.Where(f => f.UserId2 == userId).Select(f => f.UserId1);
            return await left.Concat(right).ToListAsync(ct);
        }

        public Task<int> CountMutualFriendsAsync(string userAId, string userBId, CancellationToken ct = default)
        {
            var aLeft = _db.Friendships.Where(f => f.UserId1 == userAId).Select(f => f.UserId2);
            var aRight = _db.Friendships.Where(f => f.UserId2 == userAId).Select(f => f.UserId1);
            var aFriends = aLeft.Concat(aRight);

            var bLeft = _db.Friendships.Where(f => f.UserId1 == userBId).Select(f => f.UserId2);
            var bRight = _db.Friendships.Where(f => f.UserId2 == userBId).Select(f => f.UserId1);
            var bFriends = bLeft.Concat(bRight);

            return aFriends.Intersect(bFriends).CountAsync(ct);
        }

        private static (string u1, string u2) Normalize(string a, string b)
            => string.CompareOrdinal(a, b) < 0 ? (a, b) : (b, a);
    }
}
