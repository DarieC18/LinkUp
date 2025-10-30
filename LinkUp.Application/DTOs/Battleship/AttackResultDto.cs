namespace LinkUp.Application.DTOs.Battleship
{
    public class AttackResultDto
    {
        public bool Accepted { get; set; }
        public bool IsMyTurnNow { get; set; }
        public bool IsHit { get; set; }
        public bool IsGameFinished { get; set; }
        public string? Message { get; set; }
    }
}
