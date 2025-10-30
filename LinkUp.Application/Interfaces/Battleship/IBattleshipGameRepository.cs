using LinkUp.Application.Common.Persistence;
using LinkUp.Domain.Entities.Battleship;

namespace LinkUp.Application.Interfaces.Battleship
{
    public interface IBattleshipGameRepository : IRepository<BattleshipGame>
    {
        Task<bool> ExistsActiveBetweenAsync(string userAId, string userBId);
        Task<IReadOnlyList<BattleshipGame>> ListActiveByUserAsync(string userId);
        Task<IReadOnlyList<BattleshipGame>> ListHistoryByUserAsync(string userId);
        Task<BattleshipGame?> GetActiveByPairAsync(string userAId, string userBId);
    }
}
