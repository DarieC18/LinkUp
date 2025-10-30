namespace LinkUp.Application.DTOs.Battleship
{
    public class GameHistoryItemDto
    {
        public Guid GameId { get; set; }
        public string OpponentUserName { get; set; } = default!;
        public string OpponentUserId { get; set; } = default!;
        public string StartedAt { get; set; } = default!;
        public string FinishedAt { get; set; } = default!;
        public double DurationHours { get; set; }
        public bool IWon { get; set; }
        public string WinnerDisplay { get; set; } = default!;
    }
}
