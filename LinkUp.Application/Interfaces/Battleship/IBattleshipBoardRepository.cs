using LinkUp.Domain.Entities.Battleship;

namespace LinkUp.Application.Interfaces.Battleship
{
    public interface IBattleshipBoardRepository
    {
        Task<BattleshipBoard?> GetByGameAndOwnerAsync(Guid gameId, string ownerUserId);
        Task AddAsync(BattleshipBoard board);
        Task UpdateAsync(BattleshipBoard board);
    }
}
