using LinkUp.Domain.Entities.Battleship;

namespace LinkUp.Application.Interfaces.Battleship
{
    public interface IBattleshipAttackRepository
    {
        Task AddAsync(BattleshipAttack attack);
        Task<IReadOnlyList<BattleshipAttack>> ListByGameAsync(Guid gameId);
        Task<bool> ExistsAttackAtAsync(Guid gameId, string attackerId, int row, int col);
        Task<int> GetNextTurnIndexAsync(Guid gameId);
    }
}
