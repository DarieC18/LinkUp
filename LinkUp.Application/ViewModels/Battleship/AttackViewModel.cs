namespace LinkUp.Application.ViewModels.Battleship
{
    public class AttackVm
    {
        public string GameId { get; set; } = default!;
        public int BoardSize { get; set; }
        public sbyte[,] AttackState { get; set; } = default!;
        public bool IsMyTurn { get; set; }
        public string TurnMessage { get; set; } = default!;
        public bool IsGameFinished { get; set; }
    }
}
