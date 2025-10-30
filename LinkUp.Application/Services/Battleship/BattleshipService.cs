using LinkUp.Application.DTOs.Battleship;
using LinkUp.Application.Interfaces.Battleship;
using LinkUp.Application.Interfaces.Social;
using LinkUp.Application.Interfaces.Users;
using LinkUp.Application.ViewModels.Battleship;
using LinkUp.Domain.Entities.Battleship;
using LinkUp.Domain.Enums.Battleship;
using LinkUp.Domain.Rules.Battleship;

namespace LinkUp.Application.Services.Battleship
{
    internal class BattleshipService : IBattleshipService
    {
        private readonly IBattleshipGameRepository _gameRepo;
        private readonly IBattleshipBoardRepository _boardRepo;
        private readonly IBattleshipAttackRepository _attackRepo;
        private readonly IFriendshipRepository _friendRepo;
        private readonly IUsersReadOnly _usersReadOnly;

        public BattleshipService(
            IBattleshipGameRepository gameRepo,
            IBattleshipBoardRepository boardRepo,
            IBattleshipAttackRepository attackRepo,
            IFriendshipRepository friendRepo,
            IUsersReadOnly usersReadOnly)
        {
            _gameRepo = gameRepo;
            _boardRepo = boardRepo;
            _attackRepo = attackRepo;
            _friendRepo = friendRepo;
            _usersReadOnly = usersReadOnly;
        }
        public async Task<BattleshipIndexVm> GetIndexAsync(string currentUserId)
        {
            var activeGames = await _gameRepo.ListActiveByUserAsync(currentUserId);
            var historyGames = await _gameRepo.ListHistoryByUserAsync(currentUserId);

            var allOpponentIds = activeGames
                .Select(g => g.OpponentOf(currentUserId))
                .Concat(historyGames.Select(h => h.OpponentOf(currentUserId)))
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()!
                .ToList();

            var nameMap = new Dictionary<string, string>();
            foreach (var oid in allOpponentIds)
            {
                try
                {
                    var basic = await _usersReadOnly.GetBasicAsync(oid!);
                    var display = basic?.FullName;
                    if (string.IsNullOrWhiteSpace(display))
                        display = basic?.FullName;
                    nameMap[oid!] = string.IsNullOrWhiteSpace(display) ? oid! : display!;
                }
                catch
                {
                    nameMap[oid!] = oid!;
                }
            }

            var activeDtos = activeGames.Select(g =>
            {
                var oppId = g.OpponentOf(currentUserId)!;
                return new ActiveGameListItemDto
                {
                    GameId = g.Id,
                    OpponentUserId = oppId,
                    OpponentUserName = nameMap.TryGetValue(oppId, out var dn) ? dn : (oppId ?? "Desconocido"),
                    StartedAt = g.CreatedAtUtc.ToLocalTime().ToString("g"),
                    HoursSinceStart = Math.Max(0, (DateTimeOffset.UtcNow - g.CreatedAtUtc).TotalHours),
                    IsMyTurn = g.CurrentTurnUserId == currentUserId
                };
            }).ToList();

            var historyDtos = historyGames.Select(h =>
            {
                var oppId = h.OpponentOf(currentUserId)!;
                return new GameHistoryItemDto
                {
                    GameId = h.Id,
                    OpponentUserId = oppId,
                    OpponentUserName = nameMap.TryGetValue(oppId, out var dn) ? dn : (oppId ?? "Desconocido"),
                    StartedAt = h.CreatedAtUtc.ToLocalTime().ToString("g"),
                    FinishedAt = h.FinishedAtUtc?.ToLocalTime().ToString("g") ?? "-",
                    DurationHours = h.FinishedAtUtc.HasValue
                        ? (h.FinishedAtUtc.Value - h.CreatedAtUtc).TotalHours
                        : 0,
                    IWon = h.WinnerUserId == currentUserId,
                    WinnerDisplay = h.WinnerUserId == currentUserId
                        ? "Yo"
                        : (h.WinnerUserId == null ? "-" : "Oponente")
                };
            }).ToList();

            var summary = new GameSummaryDto
            {
                TotalPlayed = historyDtos.Count,
                Won = historyDtos.Count(x => x.IWon),
                Lost = historyDtos.Count(x => !x.IWon)
            };

            return new BattleshipIndexVm
            {
                ActiveGames = activeDtos,
                History = historyDtos,
                Summary = summary
            };
        }

        public async Task<Guid> CreateGameAsync(string currentUserId, string friendUserId)
        {
            var exists = await _gameRepo.ExistsActiveBetweenAsync(currentUserId, friendUserId);
            if (exists)
                throw new InvalidOperationException("Ya existe una partida activa entre ambos jugadores.");

            var gameId = Guid.NewGuid();

            var game = new BattleshipGame
            {
                Id = gameId,
                Player1Id = currentUserId,
                Player2Id = friendUserId,
                CreatedAtUtc = DateTimeOffset.UtcNow,
                Status = BattleshipGameStatus.Placing
            };

            var board1 = new BattleshipBoard
            {
                Id = Guid.NewGuid(),
                GameId = gameId,
                OwnerUserId = currentUserId,
                IsPlacementComplete = false
            };

            var board2 = new BattleshipBoard
            {
                Id = Guid.NewGuid(),
                GameId = gameId,
                OwnerUserId = friendUserId,
                IsPlacementComplete = false
            };

            game.Boards.Add(board1);
            game.Boards.Add(board2);

            await _gameRepo.AddAsync(game);
            await _boardRepo.AddAsync(board1);
            await _boardRepo.AddAsync(board2);

            await _gameRepo.SaveChangesAsync();

            return gameId;
        }

        public async Task<BoardVm> GetBoardAsync(Guid gameId, string currentUserId)
        {
            var game = await _gameRepo.GetByIdAsync(gameId)
                ?? throw new InvalidOperationException("Partida no encontrada.");

            var board = game.Boards.FirstOrDefault(b => b.OwnerUserId == currentUserId)
                ?? throw new InvalidOperationException("No tienes un tablero asignado en esta partida.");

            var occupied = new bool[BattleshipRules.BoardSize, BattleshipRules.BoardSize];

            foreach (var sp in board.ShipPlacements)
            {
                var len = BattleshipRules.ShipLengths[sp.ShipType];
                for (int i = 0; i < len; i++)
                {
                    var (r, c) = GetNextCell(sp.OriginRow, sp.OriginCol, sp.Direction, i);
                    occupied[r, c] = true;
                }
            }

            return new BoardVm
            {
                GameId = game.Id.ToString(),
                BoardSize = BattleshipRules.BoardSize,
                Occupied = occupied,
                IsPlacementComplete = board.IsPlacementComplete,
                InfoMessage = board.IsPlacementComplete
                    ? "Has colocado todos tus barcos."
                    : "Selecciona una celda para posicionar tu barco."
            };
        }
        public async Task PlaceShipAsync(PlaceShipRequestDto dto, string currentUserId)
        {
            var game = await _gameRepo.GetByIdAsync(dto.GameId)
                ?? throw new InvalidOperationException("Partida no encontrada.");

            if (game.Status != BattleshipGameStatus.Placing)
                throw new InvalidOperationException("La partida ya no está en fase de colocación.");

            var board = game.Boards.FirstOrDefault(b => b.OwnerUserId == currentUserId)
                ?? throw new InvalidOperationException("No tienes un tablero asignado en esta partida.");

            if (board.ShipPlacements.Any(x => x.ShipType == dto.ShipType))
                throw new InvalidOperationException("Este tipo de barco ya fue colocado.");

            var length = BattleshipRules.ShipLengths[dto.ShipType];

            var cells = new List<(int r, int c)>();
            for (int i = 0; i < length; i++)
            {
                var (r, c) = GetNextCell(dto.Row, dto.Col, dto.Direction, i);
                if (r < 0 || c < 0 || r >= BattleshipRules.BoardSize || c >= BattleshipRules.BoardSize)
                    throw new InvalidOperationException("El barco no cabe en esa dirección.");
                cells.Add((r, c));
            }

            foreach (var existing in board.ShipPlacements)
            {
                var existingLength = BattleshipRules.ShipLengths[existing.ShipType];
                for (int i = 0; i < existingLength; i++)
                {
                    var (r, c) = GetNextCell(existing.OriginRow, existing.OriginCol, existing.Direction, i);
                    if (cells.Any(cc => cc.r == r && cc.c == c))
                        throw new InvalidOperationException("El barco se superpone con otro ya colocado.");
                }
            }

            var placement = new BattleshipShipPlacement
            {
                Id = Guid.NewGuid(),
                BoardId = board.Id,
                ShipType = dto.ShipType,
                OriginRow = dto.Row,
                OriginCol = dto.Col,
                Direction = dto.Direction
            };

            board.ShipPlacements.Add(placement);
            _gameRepo.MarkAdded(placement);

            var totalShipTypes = Enum.GetValues(typeof(ShipType)).Length;
            if (board.ShipPlacements.Count == totalShipTypes)
                board.IsPlacementComplete = true;

            if (game.Boards.All(b => b.IsPlacementComplete))
            {
                game.Status = BattleshipGameStatus.Attacking;
                game.CurrentTurnUserId = game.Player1Id;
            }

            await _gameRepo.SaveChangesAsync();
        }
        public async Task<bool> IsPlacementCompleteAsync(Guid gameId, string currentUserId)
        {
            var game = await _gameRepo.GetByIdAsync(gameId)
                ?? throw new InvalidOperationException("Partida no encontrada.");

            var board = game.Boards.FirstOrDefault(b => b.OwnerUserId == currentUserId)
                ?? throw new InvalidOperationException("No tienes un tablero asignado en esta partida.");

            return board.IsPlacementComplete;
        }

        private static (int r, int c) GetNextCell(int row, int col, Direction dir, int step)
        {
            return dir switch
            {
                Direction.Up => (row - step, col),     // mueve hacia arriba
                Direction.Down => (row + step, col),   // mueve hacia abajo
                Direction.Left => (row, col - step),   // mueve hacia la izquierda
                Direction.Right => (row, col + step),  // mueve hacia la derecha
                _ => (row, col)
            };
        }

        public async Task<AttackVm> GetAttackAsync(Guid gameId, string currentUserId)
        {
            var game = await _gameRepo.GetByIdAsync(gameId)
                ?? throw new InvalidOperationException("Partida no encontrada.");

            if (game.Status != BattleshipGameStatus.Attacking && game.Status != BattleshipGameStatus.Finished)
                throw new InvalidOperationException("La partida aún no ha iniciado la fase de ataque.");

            var attacks = await _attackRepo.ListByGameAsync(gameId);

            var board = new sbyte[BattleshipRules.BoardSize, BattleshipRules.BoardSize];
            for (int r = 0; r < BattleshipRules.BoardSize; r++)
                for (int c = 0; c < BattleshipRules.BoardSize; c++)
                    board[r, c] = -1;

            foreach (var atk in attacks.Where(a => a.AttackerUserId == currentUserId))
                board[atk.Row, atk.Col] = atk.IsHit ? (sbyte)1 : (sbyte)0;

            var vm = new AttackVm
            {
                GameId = game.Id.ToString(),
                BoardSize = BattleshipRules.BoardSize,
                AttackState = board,
                IsMyTurn = game.CurrentTurnUserId == currentUserId,
                IsGameFinished = game.Status == BattleshipGameStatus.Finished,
                TurnMessage = game.Status == BattleshipGameStatus.Finished
                    ? $"La partida ha terminado. {(game.WinnerUserId == currentUserId ? "Ganaste! " : "Perdiste!")}"
                    : (game.CurrentTurnUserId == currentUserId ? "Es tu turno para atacar" : "Es turno del oponente")
            };

            return vm;
        }

        public async Task<AttackResultDto> DoAttackAsync(AttackRequestDto dto, string currentUserId)
        {
            var game = await _gameRepo.GetByIdAsync(dto.GameId)
                ?? throw new InvalidOperationException("Partida no encontrada.");

            if (game.Status != BattleshipGameStatus.Attacking)
                return new AttackResultDto { Accepted = false, Message = "La partida no está en fase de ataque." };

            if (game.CurrentTurnUserId != currentUserId)
                return new AttackResultDto { Accepted = false, Message = "No es tu turno." };

            var already = await _attackRepo.ExistsAttackAtAsync(dto.GameId, currentUserId, dto.Row, dto.Col);
            if (already)
                return new AttackResultDto { Accepted = false, Message = "Ya atacaste esa celda." };

            var opponentId = game.OpponentOf(currentUserId)
                ?? throw new InvalidOperationException("No se encontró el oponente.");

            var opponentBoard = game.Boards.FirstOrDefault(b => b.OwnerUserId == opponentId)
                ?? throw new InvalidOperationException("El tablero del oponente no existe.");

            bool hit = false;
            foreach (var ship in opponentBoard.ShipPlacements)
            {
                var len = BattleshipRules.ShipLengths[ship.ShipType];
                for (int i = 0; i < len; i++)
                {
                    var (r, c) = GetNextCell(ship.OriginRow, ship.OriginCol, ship.Direction, i);
                    if (r == dto.Row && c == dto.Col)
                    {
                        hit = true;
                        break;
                    }
                }
                if (hit) break;
            }

            var turn = await _attackRepo.GetNextTurnIndexAsync(dto.GameId);
            var attack = new BattleshipAttack
            {
                Id = Guid.NewGuid(),
                GameId = dto.GameId,
                AttackerUserId = currentUserId,
                Row = dto.Row,
                Col = dto.Col,
                IsHit = hit,
                TurnIndex = turn,
                CreatedAtUtc = DateTimeOffset.UtcNow
            };
            await _attackRepo.AddAsync(attack);

            if (hit && IsOpponentDefeated(opponentBoard, game, currentUserId))
            {
                game.Status = BattleshipGameStatus.Finished;
                game.WinnerUserId = currentUserId;
                game.FinishedAtUtc = DateTimeOffset.UtcNow;
            }
            else
            {
                game.CurrentTurnUserId = opponentId;
            }

            await _gameRepo.SaveChangesAsync();

            return new AttackResultDto
            {
                Accepted = true,
                IsHit = hit,
                IsGameFinished = game.Status == BattleshipGameStatus.Finished,
                IsMyTurnNow = game.CurrentTurnUserId == currentUserId,
                Message = hit ? "¡Impacto!" : "Agua."
            };
        }

        private bool IsOpponentDefeated(BattleshipBoard opponentBoard, BattleshipGame game, string currentUserId)
        {
            var attacks = game.Attacks
                .Where(a => a.AttackerUserId == currentUserId && a.IsHit)
                .ToList();

            var hitCells = new HashSet<(int, int)>(attacks.Select(a => (a.Row, a.Col)));

            foreach (var ship in opponentBoard.ShipPlacements)
            {
                var len = BattleshipRules.ShipLengths[ship.ShipType];
                for (int i = 0; i < len; i++)
                {
                    var (r, c) = GetNextCell(ship.OriginRow, ship.OriginCol, ship.Direction, i);
                    if (!hitCells.Contains((r, c)))
                        return false;
                }
            }

            return true;
        }

        public async Task SurrenderAsync(Guid gameId, string currentUserId)
        {
            var game = await _gameRepo.GetByIdAsync(gameId)
                ?? throw new InvalidOperationException("Partida no encontrada.");

            if (game.Status == BattleshipGameStatus.Finished)
                return;

            var opponentId = game.OpponentOf(currentUserId)
                ?? throw new InvalidOperationException("No se encontró el oponente.");

            game.Status = BattleshipGameStatus.Finished;
            game.WinnerUserId = opponentId;
            game.FinishedAtUtc = DateTimeOffset.UtcNow;

            await _gameRepo.SaveChangesAsync();
        }

        public async Task<ResultVm> GetResultsAsync(Guid gameId, string currentUserId)
        {
            var game = await _gameRepo.GetByIdAsync(gameId)
                ?? throw new InvalidOperationException("Partida no encontrada.");

            if (game.Status != BattleshipGameStatus.Finished)
                throw new InvalidOperationException("La partida aún no ha finalizado.");

            var attacks = await _attackRepo.ListByGameAsync(gameId);
            var myAttacks = attacks.Where(a => a.AttackerUserId == currentUserId).ToList();

            var matrix = new sbyte[BattleshipRules.BoardSize, BattleshipRules.BoardSize];
            for (int r = 0; r < BattleshipRules.BoardSize; r++)
                for (int c = 0; c < BattleshipRules.BoardSize; c++)
                    matrix[r, c] = -1;

            foreach (var atk in myAttacks)
                matrix[atk.Row, atk.Col] = atk.IsHit ? (sbyte)1 : (sbyte)0;

            return new ResultVm
            {
                GameId = game.Id.ToString(),
                BoardSize = BattleshipRules.BoardSize,
                MyAttackState = matrix,
                WinnerDisplay = game.WinnerUserId == currentUserId ? "Yo" : "Oponente",
                StartedAt = game.CreatedAtUtc.ToLocalTime().ToString("g"),
                FinishedAt = game.FinishedAtUtc?.ToLocalTime().ToString("g") ?? "-"
            };
        }

        public async Task<OpponentBoardVm> GetOpponentBoardAsync(Guid gameId, string currentUserId)
        {
            var game = await _gameRepo.GetByIdAsync(gameId)
                ?? throw new InvalidOperationException("Partida no encontrada.");

            var opponentId = game.OpponentOf(currentUserId)
                ?? throw new InvalidOperationException("No se encontró el oponente.");

            var opponentBoard = game.Boards.FirstOrDefault(b => b.OwnerUserId == opponentId)
                ?? throw new InvalidOperationException("No se encontró el tablero del oponente.");

            var boardSize = BattleshipRules.BoardSize;

            var matrix = new sbyte[boardSize, boardSize];
            for (int r = 0; r < boardSize; r++)
                for (int c = 0; c < boardSize; c++)
                    matrix[r, c] = -1;

            if (game.Status == BattleshipGameStatus.Attacking || game.Status == BattleshipGameStatus.Finished)
            {
                var attacks = await _attackRepo.ListByGameAsync(gameId);
                var opponentAttacks = attacks.Where(a => a.AttackerUserId == opponentId);

                foreach (var atk in opponentAttacks)
                    matrix[atk.Row, atk.Col] = atk.IsHit ? (sbyte)1 : (sbyte)0;
            }

            return new OpponentBoardVm
            {
                GameId = game.Id.ToString(),
                BoardSize = boardSize,
                OpponentAttackState = matrix,
                IsOpponentReady = opponentBoard.IsPlacementComplete
            };
        }

        public async Task<MyPlacementVm> GetMyPlacementAsync(Guid gameId, string currentUserId)
        {
            var game = await _gameRepo.GetByIdAsync(gameId)
                ?? throw new InvalidOperationException("Partida no encontrada.");

            var board = game.Boards.FirstOrDefault(b => b.OwnerUserId == currentUserId)
                ?? throw new InvalidOperationException("No tienes un tablero asignado en esta partida.");

            var matrix = new bool[BattleshipRules.BoardSize, BattleshipRules.BoardSize];

            foreach (var ship in board.ShipPlacements)
            {
                var len = BattleshipRules.ShipLengths[ship.ShipType];
                for (int i = 0; i < len; i++)
                {
                    var (r, c) = GetNextCell(ship.OriginRow, ship.OriginCol, ship.Direction, i);
                    matrix[r, c] = true;
                }
            }

            var placed = board.ShipPlacements.Select(p => p.ShipType).ToHashSet();

            var pending = Enum.GetValues(typeof(ShipType))
                .Cast<ShipType>()
                .Where(t => !placed.Contains(t))
                .Select(t => t.ToString())
                .ToList();

            return new MyPlacementVm
            {
                GameId = game.Id.ToString(),
                BoardSize = BattleshipRules.BoardSize,
                MyShips = matrix,
                PendingShips = pending,
                InfoMessage = pending.Count == 0
                    ? "Has colocado todos tus barcos."
                    : "Selecciona un barco y posiciónalo en el tablero."
            };
        }

        public async Task<IReadOnlyList<FriendOptionDto>> ListFriendsAvailableForGameAsync(string currentUserId)
        {
            var friendIds = await _friendRepo.ListFriendIdsAsync(currentUserId);

            if (friendIds == null || friendIds.Count == 0)
                return Array.Empty<FriendOptionDto>();

            var result = new List<FriendOptionDto>(friendIds.Count);

            foreach (var friendId in friendIds)
            {
                var existsActive = await _gameRepo.ExistsActiveBetweenAsync(currentUserId, friendId);
                if (existsActive)
                    continue;

                var basic = await _usersReadOnly.GetBasicAsync(friendId);
                var displayName = basic?.FullName ?? friendId;
                var avatar = basic?.AvatarPath;

                int mutual = 0;
                try
                {
                    mutual = await _friendRepo.CountMutualFriendsAsync(currentUserId, friendId);
                }
                catch
                {
                    mutual = 0;
                }

                result.Add(new FriendOptionDto
                {
                    UserId = friendId,
                    UserName = displayName,
                    AvatarUrl = avatar,
                    MutualFriends = mutual
                });
            }

            return result
                .OrderByDescending(x => x.MutualFriends)
                .ThenBy(x => x.UserName)
                .ToList();
        }

        public async Task<SelectShipVm> GetSelectShipAsync(Guid gameId, string currentUserId)
        {
            var game = await _gameRepo.GetByIdAsync(gameId)
                ?? throw new InvalidOperationException("Partida no encontrada.");

            var myBoard = game.Boards.FirstOrDefault(b => b.OwnerUserId == currentUserId)
                ?? throw new InvalidOperationException("No tienes un tablero asignado en esta partida.");

            var placed = myBoard.ShipPlacements.Select(p => p.ShipType).ToHashSet();

            var pending = Enum.GetValues(typeof(ShipType))
                .Cast<ShipType>()
                .Where(t => !placed.Contains(t))
                .ToList();

            return new SelectShipVm
            {
                GameId = game.Id.ToString(),
                PendingShips = pending
            };
        }
    }
}
