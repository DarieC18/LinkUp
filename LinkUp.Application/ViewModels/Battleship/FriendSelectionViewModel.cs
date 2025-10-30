using LinkUp.Application.DTOs.Battleship;

namespace LinkUp.Application.ViewModels.Battleship
{
    public class FriendSelectionVm
    {
        public string? Search { get; set; }
        public IReadOnlyList<FriendOptionDto> Friends { get; set; } = new List<FriendOptionDto>();
        public string? Error { get; set; }
    }
}
