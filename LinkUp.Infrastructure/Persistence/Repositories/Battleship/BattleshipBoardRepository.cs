using LinkUp.Application.Interfaces.Battleship;
using LinkUp.Domain.Entities.Battleship;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Persistence.Repositories.Battleship
{
    public class BattleshipBoardRepository : IBattleshipBoardRepository
    {
        private readonly ApplicationDbContext _db;

        public BattleshipBoardRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<BattleshipBoard?> GetByGameAndOwnerAsync(Guid gameId, string ownerUserId)
        {
            return await _db.BattleshipBoards
                .Include(x => x.ShipPlacements)
                .FirstOrDefaultAsync(x => x.GameId == gameId && x.OwnerUserId == ownerUserId);
        }

        public async Task AddAsync(BattleshipBoard board)
        {
            await _db.BattleshipBoards.AddAsync(board);
        }
        public Task UpdateAsync(BattleshipBoard board)
        {
            return Task.CompletedTask;
        }
    }
}
