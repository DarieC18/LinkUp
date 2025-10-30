namespace LinkUp.Application.ViewModels.Battleship
{
    public class ResultVm
    {
        public string GameId { get; set; } = default!;
        public int BoardSize { get; set; }
        public sbyte[,] MyAttackState { get; set; } = default!;
        public string WinnerDisplay { get; set; } = default!;
        public string StartedAt { get; set; } = default!;
        public string FinishedAt { get; set; } = default!;
    }
}
