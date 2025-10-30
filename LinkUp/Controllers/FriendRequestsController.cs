using LinkUp.Application.Interfaces.Social;
using LinkUp.Application.Interfaces.Users;
using LinkUp.Application.ViewModels.Friends;
using LinkUp.Application.ViewModels.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public sealed class FriendRequestsController : Controller
{
    private readonly IFriendsService _friends;
    private readonly ICurrentUser _current;

    public FriendRequestsController(IFriendsService friends, ICurrentUser current)
    {
        _friends = friends;
        _current = current;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var userId = _current.UserId!;
        var vm = new FriendRequestsIndexVm
        {
            Received = await _friends.GetPendingReceivedAsync(userId, ct),
            Sent = await _friends.GetSentAsync(userId, ct)
        };
        return View(vm);
    }

    [HttpGet]
    public IActionResult Accept(Guid id) => View("Confirm", new ConfirmActionVm
    {
        Title = "Aceptar solicitud",
        Message = "¿Deseas aceptar esta solicitud?",
        PostAction = nameof(AcceptConfirmed),
        Controller = "FriendRequests",
        ReturnTo = nameof(Index),
        Id = id.ToString()
    });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> AcceptConfirmed(string id, CancellationToken ct)
    {
        await _friends.AcceptAsync(Guid.Parse(id), _current.UserId!, ct);
        TempData["Info"] = "Solicitud aceptada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Reject(Guid id) => View("Confirm", new ConfirmActionVm
    {
        Title = "Rechazar solicitud",
        Message = "¿Deseas rechazar esta solicitud?",
        PostAction = nameof(RejectConfirmed),
        Controller = "FriendRequests",
        ReturnTo = nameof(Index),
        Id = id.ToString()
    });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectConfirmed(string id, CancellationToken ct)
    {
        await _friends.RejectAsync(Guid.Parse(id), _current.UserId!, ct);
        TempData["Info"] = "Solicitud rechazada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Cancel(Guid id) => View("Confirm", new ConfirmActionVm
    {
        Title = "Cancelar solicitud",
        Message = "¿Deseas cancelar tu solicitud enviada?",
        PostAction = nameof(CancelConfirmed),
        Controller = "FriendRequests",
        ReturnTo = nameof(Index),
        Id = id.ToString()
    });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelConfirmed(string id, CancellationToken ct)
    {
        await _friends.CancelAsync(Guid.Parse(id), _current.UserId!, ct);
        TempData["Info"] = "Solicitud cancelada.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> New(string? search, CancellationToken ct)
    {
        var userId = _current.UserId!;
        var list = await _friends.GetUsersAvailableToRequestAsync(userId, search, ct);
        ViewBag.Search = search;
        return View(list);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> SendRequest(string selectedUserId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(selectedUserId))
        {
            TempData["Error"] = "Debes seleccionar un usuario.";
            return RedirectToAction(nameof(New));
        }

        try
        {
            await _friends.CreateRequestAsync(_current.UserId!, selectedUserId, ct);
            TempData["Info"] = "Solicitud de amistad enviada correctamente.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }
}
