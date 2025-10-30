namespace LinkUp.Application.ViewModels.Battleship
{
    public class BoardVm
    {
        public string GameId { get; set; } = default!;
        public int BoardSize { get; set; }
        public bool[,] Occupied { get; set; } = default!;
        public bool IsPlacementComplete { get; set; }
        public string? InfoMessage { get; set; }
    }
}
