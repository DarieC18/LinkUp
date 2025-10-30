namespace LinkUp.Domain.Entities.Battleship
{

    public class BattleshipBoard
    {
        public Guid Id { get; set; }

        public Guid GameId { get; set; }
        public BattleshipGame Game { get; set; } = default!;

        public string OwnerUserId { get; set; } = default!;

        public bool IsPlacementComplete { get; set; }

        public string? CellsCompressed { get; set; }

        public ICollection<BattleshipShipPlacement> ShipPlacements { get; set; } =
            new List<BattleshipShipPlacement>();
    }
}
