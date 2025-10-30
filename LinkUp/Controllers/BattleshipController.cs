using LinkUp.Application.DTOs.Battleship;
using LinkUp.Application.Interfaces.Battleship;
using LinkUp.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinkUp.Controllers
{
    [Authorize]
    public class BattleshipController : Controller
    {
        private readonly IBattleshipService _battleshipService;
        private readonly UserManager<AppUser> _userManager;

        public BattleshipController(
            IBattleshipService battleshipService,
            UserManager<AppUser> userManager)
        {
            _battleshipService = battleshipService;
            _userManager = userManager;
        }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        public async Task<IActionResult> Index()
        {
            var vm = await _battleshipService.GetIndexAsync(CurrentUserId);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> New()
        {
            var friends = await _battleshipService.ListFriendsAvailableForGameAsync(CurrentUserId);
            return View(friends);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> New(CreateGameRequestDto req)
        {
            if (string.IsNullOrWhiteSpace(req.FriendUserId))
            {
                TempData["Error"] = "Debes seleccionar un amigo para iniciar la partida.";
                return RedirectToAction(nameof(New));
            }

            var id = await _battleshipService.CreateGameAsync(CurrentUserId, req.FriendUserId);
            TempData["Info"] = "Partida creada correctamente. ¡Coloca tus barcos!";
            return RedirectToAction(nameof(SelectShip), new { gameId = id });
        }

        [HttpGet]
        public async Task<IActionResult> Enter(Guid gameId)
        {
            var userId = _userManager.GetUserId(User)!;

            var pending = await _battleshipService.GetSelectShipAsync(gameId, userId);
            if (pending.PendingShips?.Any() == true)
            {
                var nextShip = pending.PendingShips.First();
                return RedirectToAction("MyPlacement", new { gameId, ship = nextShip });
            }

            var opp = await _battleshipService.GetOpponentBoardAsync(gameId, userId);
            if (!opp.IsOpponentReady)
            {
                TempData["Info"] = "Esperando a que tu oponente complete la colocación.";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Attack", new { gameId });
        }

        [HttpGet]
        public async Task<IActionResult> SelectShip(Guid gameId)
        {
            var vm = await _battleshipService.GetSelectShipAsync(gameId, CurrentUserId);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Board(Guid gameId)
        {
            var vm = await _battleshipService.GetBoardAsync(gameId, CurrentUserId);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> MyPlacement(Guid gameId, string ship)
        {
            var userId = _userManager.GetUserId(User)!;
            var vm = await _battleshipService.GetMyPlacementAsync(gameId, userId);
            ViewBag.SelectedShip = ship;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceShip(PlaceShipRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = _userManager.GetUserId(User)!;
            await _battleshipService.PlaceShipAsync(dto, userId);

            var pending = await _battleshipService.GetSelectShipAsync(dto.GameId, userId);
            if (pending.PendingShips?.Any() == true)
                return RedirectToAction("MyPlacement", new { gameId = dto.GameId, ship = pending.PendingShips.First() });

            return RedirectToAction("Attack", new { gameId = dto.GameId });
        }

        [HttpGet]
        public async Task<IActionResult> Attack(Guid gameId)
        {
            var userId = _userManager.GetUserId(User)!;
            try
            {
                var vm = await _battleshipService.GetAttackAsync(gameId, userId);
                return View(vm);
            }
            catch (InvalidOperationException)
            {
                return RedirectToAction("Enter", new { gameId });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoAttack(AttackRequestDto dto)
        {
            var result = await _battleshipService.DoAttackAsync(dto, CurrentUserId);
            if (!result.Accepted)
            {
                TempData["Error"] = result.Message;
            }
            return RedirectToAction(nameof(Attack), new { gameId = dto.GameId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Surrender(Guid gameId)
        {
            await _battleshipService.SurrenderAsync(gameId, CurrentUserId);
            TempData["Info"] = "Te has rendido. Tu oponente gana la partida.";
            return RedirectToAction(nameof(Results), new { gameId });
        }


        [HttpGet]
        public async Task<IActionResult> Results(Guid gameId)
        {
            var vm = await _battleshipService.GetResultsAsync(gameId, CurrentUserId);
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> OpponentBoard(Guid gameId)
        {
            try
            {
                await _battleshipService.GetResultsAsync(gameId, CurrentUserId);
            }
            catch
            {
                TempData["Error"] = "El tablero del oponente solo está disponible cuando la partida ha finalizado.";
                return RedirectToAction(nameof(Attack), new { gameId });
            }

            var vm = await _battleshipService.GetOpponentBoardAsync(gameId, CurrentUserId);
            return View(vm);
        }
    }
}
