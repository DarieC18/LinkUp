using LinkUp.Domain.Enums.Battleship;

namespace LinkUp.Domain.Rules.Battleship
{
    public static class BattleshipRules
    {
        public const int BoardSize = 12;
        public const int InactivityTimeoutHours = 48;

        public static readonly IReadOnlyDictionary<ShipType, int> ShipLengths =
            new Dictionary<ShipType, int>
            {
                [ShipType.Size2] = 2,
                [ShipType.Size3A] = 3,
                [ShipType.Size3B] = 3,
                [ShipType.Size4] = 4,
                [ShipType.Size5] = 5
            };
    }
}
