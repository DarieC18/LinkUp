using LinkUp.Domain.Enums.Battleship;

namespace LinkUp.Application.ViewModels.Battleship
{
    public class SelectShipVm
    {
        public IReadOnlyList<ShipType> PendingShips { get; set; } = new List<ShipType>();
        public string GameId { get; set; } = default!;
    }
}
