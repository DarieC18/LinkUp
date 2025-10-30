namespace LinkUp.Application.ViewModels.Battleship
{
    public class OpponentBoardVm
    {
        public string GameId { get; set; } = default!;
        public int BoardSize { get; set; }
        public sbyte[,] OpponentAttackState { get; set; } = default!;
        public bool IsOpponentReady { get; set; }
    }
}
