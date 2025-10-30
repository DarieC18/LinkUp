using LinkUp.Application.DTOs.Social;
using LinkUp.Application.Interfaces.Social;
using LinkUp.Application.Interfaces.Users;
using LinkUp.Application.ViewModels.Friends;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
[Authorize]
public sealed class FriendsController : Controller
{
    private readonly IFriendsService _friends;
    private readonly IPostService _posts;
    private readonly ICurrentUser _current;
    private readonly IUsersReadOnly _users;
    private readonly IFriendshipRepository _friendships;

    public FriendsController(
        IFriendsService friends,
        IPostService posts,
        ICurrentUser current,
        IUsersReadOnly users,
        IFriendshipRepository friendships)
    {
        _friends = friends;
        _posts = posts;
        _current = current;
        _users = users;
        _friendships = friendships;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        var userId = _current.UserId!;
        var friends = await _friends.ListFriendsAsync(userId, ct);
        var feed = await _posts.GetFeedByFriendsAsync(userId, page, pageSize, ct);

        var vm = new FriendsIndexVm
        {
            Friends = friends,
            Posts = feed.Items.ToList()
        };
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(string id, CancellationToken ct)
    {
        await _friends.RemoveFriendAsync(_current.UserId!, id, ct);
        TempData["Info"] = "Amigo eliminado.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Friends/Posts/{idOrUsername}")]
    public async Task<IActionResult> Posts(string idOrUsername, int page = 1, int pageSize = 10, CancellationToken ct = default)
    {
        var currentUserId = _current.UserId!;
        string? friendId;

        if (Guid.TryParse(idOrUsername, out _))
            friendId = idOrUsername;
        else
            friendId = await _users.GetIdByUserNameAsync(idOrUsername, ct);

        if (friendId is null) return NotFound("Usuario no encontrado.");

        var areFriends = await _friendships.AreFriendsAsync(currentUserId, friendId, ct);
        if (!areFriends && currentUserId != friendId) return Forbid();

        var ub = await _users.GetBasicAsync(friendId, ct);
        ViewBag.FriendName = ub?.FullName ?? "Usuario";

        var feed = await _posts.GetFeedAsync(new GetFeedRequest
        {
            CurrentUserId = currentUserId,
            Page = page,
            PageSize = pageSize,
            UserIdFilter = friendId
        });

        return View("FriendPosts", feed);
    }

    [HttpGet]
    public IActionResult Feed() => View();
}
