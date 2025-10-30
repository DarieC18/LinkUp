using LinkUp.Application.DTOs.Battleship;
using LinkUp.Application.ViewModels.Battleship;

namespace LinkUp.Application.Interfaces.Battleship
{
    public interface IBattleshipService
    {
        Task<BattleshipIndexVm> GetIndexAsync(string currentUserId);
        Task<Guid> CreateGameAsync(string currentUserId, string friendUserId);
        Task<SelectShipVm> GetSelectShipAsync(Guid gameId, string currentUserId);
        Task<BoardVm> GetBoardAsync(Guid gameId, string currentUserId);
        Task PlaceShipAsync(PlaceShipRequestDto dto, string currentUserId);
        Task<bool> IsPlacementCompleteAsync(Guid gameId, string currentUserId);
        Task<AttackVm> GetAttackAsync(Guid gameId, string currentUserId);
        Task<AttackResultDto> DoAttackAsync(AttackRequestDto dto, string currentUserId);
        Task SurrenderAsync(Guid gameId, string currentUserId);
        Task<ResultVm> GetResultsAsync(Guid gameId, string currentUserId);
        Task<OpponentBoardVm> GetOpponentBoardAsync(Guid gameId, string currentUserId);
        Task<MyPlacementVm> GetMyPlacementAsync(Guid gameId, string currentUserId);
        Task<IReadOnlyList<FriendOptionDto>> ListFriendsAvailableForGameAsync(string currentUserId);
    }
}
