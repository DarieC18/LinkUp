namespace LinkUp.Application.DTOs.Battleship
{
    public class AttackRequestDto
    {
        public Guid GameId { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
    }
}
