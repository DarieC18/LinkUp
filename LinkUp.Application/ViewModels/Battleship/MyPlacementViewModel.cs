namespace LinkUp.Application.ViewModels.Battleship
{
    public class MyPlacementVm
    {
        public string GameId { get; set; } = default!;
        public int BoardSize { get; set; }
        public bool[,] MyShips { get; set; } = default!;

        public string? InfoMessage { get; set; }
        public IEnumerable<string>? PendingShips { get; set; }
    }
}
