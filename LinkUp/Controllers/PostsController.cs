using LinkUp.Application.DTOs.Social;
using LinkUp.Application.Interfaces.Social;
using LinkUp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinkUp.Web.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
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
                Page = page,
                PageSize = 10
            });

            return View(feed);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePostRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.Content))
                ModelState.AddModelError(nameof(model.Content), "El contenido es obligatorio.");

            var media = (model.MediaType ?? string.Empty).ToLowerInvariant();

            if (media == "image")
            {
                if (model.ImageFile is null || model.ImageFile.Length == 0)
                    ModelState.AddModelError(nameof(model.ImageFile), "Debes adjuntar una imagen.");
                model.YouTubeUrl = null;
            }
            else if (media == "video")
            {
                if (string.IsNullOrWhiteSpace(model.YouTubeUrl))
                    ModelState.AddModelError(nameof(model.YouTubeUrl), "Debes pegar un enlace de YouTube.");
                model.ImageFile = null;
            }
            else
            {
                ModelState.AddModelError(nameof(model.MediaType), "Selecciona 'Imagen' o 'Video'.");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Revisa el formulario de publicación.";
                return RedirectToAction("Index", "Home");
            }

            await _postService.CreateAsync(model);
            TempData["Info"] = "Publicación creada.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> React(Guid id, string type)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var rt = type?.ToLowerInvariant() == "like" ? ReactionType.Like : ReactionType.Dislike;

            var result = await _postService.ToggleReactionAsync(new ToggleReactionRequest
            {
                PostId = id,
                UserId = userId,
                ReactionType = rt
            });

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Json(result);

            TempData["Info"] = "Reacción actualizada";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            try
            {
                var dto = await _postService.GetForEditAsync(id, userId);
                var vm = new EditPostRequest
                {
                    PostId = dto.Id,
                    Content = dto.Content,
                    YouTubeUrl = dto.YouTubeVideoId != null ? $"https://www.youtube.com/watch?v={dto.YouTubeVideoId}" : null
                };
                ViewBag.CurrentImage = dto.ImagePath;
                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditPostRequest model)
        {
            model.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            try
            {
                await _postService.EditAsync(model);
                TempData["Info"] = "Publicación actualizada.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            try
            {
                await _postService.DeleteAsync(new DeletePostRequest { PostId = id, UserId = userId });
                TempData["Info"] = "Publicación eliminada.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index", "Home");
        }

    }
}
