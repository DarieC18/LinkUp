using LinkUp.Application.DTOs.Battleship;

namespace LinkUp.Application.ViewModels.Battleship
{
    public class BattleshipIndexVm
    {
        public IReadOnlyList<ActiveGameListItemDto> ActiveGames { get; set; } = new List<ActiveGameListItemDto>();
        public IReadOnlyList<GameHistoryItemDto> History { get; set; } = new List<GameHistoryItemDto>();
        public GameSummaryDto Summary { get; set; } = new GameSummaryDto();
    }
}
