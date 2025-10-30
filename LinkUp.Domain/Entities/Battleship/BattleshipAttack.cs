namespace LinkUp.Domain.Entities.Battleship
{

    public class BattleshipAttack
    {
        public Guid Id { get; set; }

        public Guid GameId { get; set; }
        public BattleshipGame Game { get; set; } = default!;
        public string AttackerUserId { get; set; } = default!;
        public int Row { get; set; }
        public int Col { get; set; }
        public bool IsHit { get; set; }
        public int TurnIndex { get; set; }
        public DateTimeOffset CreatedAtUtc { get; set; }
    }
}
