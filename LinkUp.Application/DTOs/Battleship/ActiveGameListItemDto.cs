namespace LinkUp.Application.DTOs.Battleship
{
    public class ActiveGameListItemDto
    {
        public Guid GameId { get; set; }
        public string OpponentUserName { get; set; } = default!;
        public string OpponentUserId { get; set; } = default!;
        public string StartedAt { get; set; } = default!;
        public double HoursSinceStart { get; set; }
        public bool IsMyTurn { get; set; }
    }
}
