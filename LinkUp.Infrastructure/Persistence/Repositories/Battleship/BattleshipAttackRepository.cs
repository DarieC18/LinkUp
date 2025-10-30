using LinkUp.Application.Interfaces.Battleship;
using LinkUp.Domain.Entities.Battleship;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Persistence.Repositories.Battleship
{
    public class BattleshipAttackRepository : IBattleshipAttackRepository
    {
        private readonly ApplicationDbContext _db;

        public BattleshipAttackRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(BattleshipAttack attack)
        {
            await _db.BattleshipAttacks.AddAsync(attack);
        }

        public async Task<IReadOnlyList<BattleshipAttack>> ListByGameAsync(Guid gameId)
        {
            return await _db.BattleshipAttacks
                .Where(x => x.GameId == gameId)
                .OrderBy(x => x.TurnIndex)
                .ToListAsync();
        }

        public async Task<bool> ExistsAttackAtAsync(Guid gameId, string attackerId, int row, int col)
        {
            return await _db.BattleshipAttacks.AnyAsync(x =>
                x.GameId == gameId && x.AttackerUserId == attackerId &&
                x.Row == row && x.Col == col);
        }

        public async Task<int> GetNextTurnIndexAsync(Guid gameId)
        {
            var last = await _db.BattleshipAttacks
                .Where(x => x.GameId == gameId)
                .OrderByDescending(x => x.TurnIndex)
                .Select(x => x.TurnIndex)
                .FirstOrDefaultAsync();

            return last + 1;
        }
    }
}
