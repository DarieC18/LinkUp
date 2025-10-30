using LinkUp.Application.Interfaces.Social;
using LinkUp.Application.Interfaces.Users;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Web.ViewComponents
{
    public sealed class PendingRequestsBadgeViewComponent : ViewComponent
    {
        private readonly IFriendsService _friends;
        private readonly ICurrentUser _current;

        public PendingRequestsBadgeViewComponent(IFriendsService friends, ICurrentUser current)
        {
            _friends = friends;
            _current = current;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!_current.IsAuthenticated) return Content(string.Empty);
            var count = await _friends.CountPendingAsync(_current.UserId!); ;
            return View(count);
        }
    }
}
