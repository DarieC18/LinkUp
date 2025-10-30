using LinkUp.Application.DTOs.Social;
using LinkUp.Application.Interfaces.Social;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LinkUp.Web.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ICommentService _service;
        public CommentsController(ICommentService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> Thread(Guid postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var data = await _service.GetThreadAsync(postId, userId);

            var ctx = (string?)Request.Query["ctx"];
            ViewData["AllowCommentDelete"] = string.IsNullOrEmpty(ctx) || ctx != "friends";

            return PartialView("~/Views/Shared/_CommentsThread.cshtml", data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCommentRequest req)
        {
            req.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _service.AddCommentAsync(req);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Ok(new { ok = true });
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(CreateReplyRequest req)
        {
            req.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _service.AddReplyAsync(req);
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return Ok(new { ok = true });
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditCommentRequest req)
        {
            req.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            try
            {
                await _service.EditAsync(req);
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") return Ok(new { ok = true });
                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest") return BadRequest(new { error = ex.Message });
                TempData["Error"] = ex.Message;
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] DeleteCommentRequest req)
        {
            req.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            if (req.CommentId == Guid.Empty && Request.HasFormContentType)
            {
                if (Guid.TryParse(Request.Form["Id"], out var formId) && formId != Guid.Empty)
                    req.CommentId = formId;
            }

            if (req.CommentId == Guid.Empty)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return BadRequest(new { error = "Comment id is required." });

                TempData["Error"] = "No se pudo identificar el comentario a eliminar.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            try
            {
                await _service.DeleteAsync(req);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Ok(new { ok = true });

                TempData["Info"] = "Comentario eliminado.";
                return Redirect(Request.Headers["Referer"].ToString());
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return BadRequest(new { error = ex.Message });

                TempData["Error"] = ex.Message;
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }
    }
}
