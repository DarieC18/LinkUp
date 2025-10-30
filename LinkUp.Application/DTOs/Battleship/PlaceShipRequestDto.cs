using LinkUp.Domain.Enums.Battleship;
using LinkUp.Domain.Rules.Battleship;
using System.ComponentModel.DataAnnotations;

public class PlaceShipRequestDto
{
    public Guid GameId { get; set; }

    [Required]
    public ShipType ShipType { get; set; }

    [Range(0, BattleshipRules.BoardSize - 1)]
    public int Row { get; set; }

    [Range(0, BattleshipRules.BoardSize - 1)]
    public int Col { get; set; }

    [Required]
    public Direction Direction { get; set; }
}
