using LinkUp.Application.Interfaces.Battleship;
using LinkUp.Domain.Entities.Battleship;
using LinkUp.Domain.Enums.Battleship;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Persistence.Repositories.Battleship
{
    public class BattleshipGameRepository
        : Repository<BattleshipGame>, IBattleshipGameRepository
    {
        private readonly ApplicationDbContext _db;

        public BattleshipGameRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public new async Task<BattleshipGame?> GetByIdAsync(Guid id)
        {
            return await _db.BattleshipGames
                .AsTracking()
                .AsSplitQuery()
                .Include(g => g.Boards)
                    .ThenInclude(b => b.ShipPlacements)
                .Include(g => g.Attacks)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<bool> ExistsActiveBetweenAsync(string userAId, string userBId)
        {
            return await _db.BattleshipGames.AnyAsync(g =>
                ((g.Player1Id == userAId && g.Player2Id == userBId) ||
                 (g.Player1Id == userBId && g.Player2Id == userAId)) &&
                g.Status != BattleshipGameStatus.Finished);
        }

        public async Task<BattleshipGame?> GetActiveByPairAsync(string userAId, string userBId)
        {
            return await _db.BattleshipGames
                .AsTracking()
                .AsSplitQuery()
                .Include(g => g.Boards).ThenInclude(b => b.ShipPlacements)
                .Include(g => g.Attacks)
                .FirstOrDefaultAsync(g =>
                    ((g.Player1Id == userAId && g.Player2Id == userBId) ||
                     (g.Player1Id == userBId && g.Player2Id == userAId)) &&
                    g.Status != BattleshipGameStatus.Finished);
        }

        public async Task<IReadOnlyList<BattleshipGame>> ListActiveByUserAsync(string userId)
        {
            return await _db.BattleshipGames
                .Where(g => (g.Player1Id == userId || g.Player2Id == userId) &&
                            g.Status != BattleshipGameStatus.Finished)
                .OrderByDescending(g => g.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<BattleshipGame>> ListHistoryByUserAsync(string userId)
        {
            return await _db.BattleshipGames
                .Where(g => (g.Player1Id == userId || g.Player2Id == userId) &&
                            g.Status == BattleshipGameStatus.Finished)
                .OrderByDescending(g => g.FinishedAtUtc)
                .ToListAsync();
        }
    }
}
