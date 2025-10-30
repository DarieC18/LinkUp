using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkUp.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitBattleshipModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BattleshipGames",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Player1Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Player2Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    FinishedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CurrentTurnUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WinnerUserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleshipGames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BattleshipAttacks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttackerUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Row = table.Column<int>(type: "int", nullable: false),
                    Col = table.Column<int>(type: "int", nullable: false),
                    IsHit = table.Column<bool>(type: "bit", nullable: false),
                    TurnIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedAtUtc = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleshipAttacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BattleshipAttacks_BattleshipGames_GameId",
                        column: x => x.GameId,
                        principalTable: "BattleshipGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BattleshipBoards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    IsPlacementComplete = table.Column<bool>(type: "bit", nullable: false),
                    CellsCompressed = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleshipBoards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BattleshipBoards_BattleshipGames_GameId",
                        column: x => x.GameId,
                        principalTable: "BattleshipGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BattleshipShipPlacements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BoardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShipType = table.Column<int>(type: "int", nullable: false),
                    OriginRow = table.Column<int>(type: "int", nullable: false),
                    OriginCol = table.Column<int>(type: "int", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleshipShipPlacements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BattleshipShipPlacements_BattleshipBoards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "BattleshipBoards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BattleshipAttacks_GameId_AttackerUserId_Row_Col",
                table: "BattleshipAttacks",
                columns: new[] { "GameId", "AttackerUserId", "Row", "Col" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BattleshipBoards_GameId_OwnerUserId",
                table: "BattleshipBoards",
                columns: new[] { "GameId", "OwnerUserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BattleshipGames_Player1Id_Player2Id_Status",
                table: "BattleshipGames",
                columns: new[] { "Player1Id", "Player2Id", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BattleshipShipPlacements_BoardId_ShipType",
                table: "BattleshipShipPlacements",
                columns: new[] { "BoardId", "ShipType" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BattleshipAttacks");

            migrationBuilder.DropTable(
                name: "BattleshipShipPlacements");

            migrationBuilder.DropTable(
                name: "BattleshipBoards");

            migrationBuilder.DropTable(
                name: "BattleshipGames");
        }
    }
}
