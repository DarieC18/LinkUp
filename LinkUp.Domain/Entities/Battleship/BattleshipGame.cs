using LinkUp.Domain.Enums.Battleship;

namespace LinkUp.Domain.Entities.Battleship
{

    public class BattleshipGame
    {
        public Guid Id { get; set; }

        public string Player1Id { get; set; } = default!;
        public string Player2Id { get; set; } = default!;

        public DateTimeOffset CreatedAtUtc { get; set; }
        public DateTimeOffset? FinishedAtUtc { get; set; }

        public BattleshipGameStatus Status { get; set; }

        public string? CurrentTurnUserId { get; set; }
        public string? WinnerUserId { get; set; }

        // Navegación
        public ICollection<BattleshipBoard> Boards { get; set; } = new List<BattleshipBoard>();
        public ICollection<BattleshipAttack> Attacks { get; set; } = new List<BattleshipAttack>();

        public bool IsActive => Status == BattleshipGameStatus.New
                                || Status == BattleshipGameStatus.Placing
                                || Status == BattleshipGameStatus.Attacking;

        public bool IsFinished => Status == BattleshipGameStatus.Finished;

        public bool BelongsTo(string userId) => Player1Id == userId || Player2Id == userId;

        public string? OpponentOf(string userId)
            => Player1Id == userId ? Player2Id
             : Player2Id == userId ? Player1Id
             : null;
    }
}
