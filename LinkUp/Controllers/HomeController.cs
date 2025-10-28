using LinkUp.Application.DTOs.Social;
using LinkUp.Application.Interfaces.Social;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinkUp.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IPostService _postService;

        public HomeController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var feed = await _postService.GetFeedAsync(new GetFeedRequest
            {
                CurrentUserId = userId,
                UserIdFilter = userId,
                Page = page,
                PageSize = 15
            });

            return View(feed);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePostRequest model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Revisa el formulario de publicación.";
                return RedirectToAction("Index");
            }

            await _postService.CreateAsync(model);
            TempData["Info"] = "Publicación creada.";
            return RedirectToAction("Index");
        }
    }
}
