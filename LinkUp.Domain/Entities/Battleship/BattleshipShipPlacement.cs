using LinkUp.Domain.Enums.Battleship;

namespace LinkUp.Domain.Entities.Battleship
{
    public class BattleshipShipPlacement
    {
        public Guid Id { get; set; }

        public Guid BoardId { get; set; }
        public BattleshipBoard Board { get; set; } = default!;

        public ShipType ShipType { get; set; }
        public int OriginRow { get; set; }
        public int OriginCol { get; set; }

        public Direction Direction { get; set; }
    }
}
