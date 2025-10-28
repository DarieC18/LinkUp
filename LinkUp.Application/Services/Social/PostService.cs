using LinkUp.Application.DTOs.Social;
using LinkUp.Application.Interfaces.Social;
using LinkUp.Application.Interfaces.Storage;
using LinkUp.Application.Interfaces.Users;
using LinkUp.Domain.Entities.Social;
using LinkUp.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace LinkUp.Application.Services.Social
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _posts;
        private readonly IReactionRepository _reactions;
        private readonly IUsersReadOnly _users;
        private readonly IFileStorage _files;
        private readonly ICurrentUser _current;

        public PostService(
            IPostRepository posts,
            IReactionRepository reactions,
            IUsersReadOnly users,
            IFileStorage files,
            ICurrentUser current)
        {
            _posts = posts;
            _reactions = reactions;
            _users = users;
            _files = files;
            _current = current;
        }
        public async Task<Guid> CreateAsync(CreatePostRequest req)
        {
            var userId = _current.UserId ?? throw new InvalidOperationException("Usuario no autenticado.");
            if (string.IsNullOrWhiteSpace(userId)) throw new InvalidOperationException("Usuario no autenticado.");

            var post = new Post
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Content = req.Content.Trim(),
                CreatedAtUtc = DateTime.UtcNow
            };

            var mt = (req.MediaType ?? "").ToLowerInvariant();

            if (mt == "image")
            {
                var path = await _files.SavePostImageAsync(req.ImageFile!, 5_000_000);
                post.ImagePath = path;
                post.YouTubeVideoId = null;
            }
            else if (mt == "video")
            {
                post.YouTubeVideoId = ExtractYouTubeId(req.YouTubeUrl!);
                if (string.IsNullOrEmpty(post.YouTubeVideoId))
                    throw new ValidationException("La URL de YouTube no es válida.");
                post.ImagePath = null;
            }

            if (!string.IsNullOrEmpty(post.ImagePath) && !string.IsNullOrEmpty(post.YouTubeVideoId))
                throw new ValidationException("Una publicación no puede tener imagen y video a la vez.");

            await _posts.AddAsync(post);
            await _posts.SaveChangesAsync();
            return post.Id;
        }

        private static string? ExtractYouTubeId(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;
            var m = Regex.Match(url,
                @"(?:youtu\.be/|youtube\.com/(?:watch\?v=|embed/|shorts/))([A-Za-z0-9_-]{11})",
                RegexOptions.IgnoreCase);
            return m.Success ? m.Groups[1].Value : null;
        }

        public async Task EditAsync(EditPostRequest req)
        {
            var post = await _posts.GetByIdAsync(req.PostId)
                       ?? throw new InvalidOperationException("Publicación no encontrada.");

            if (post.UserId != req.UserId)
                throw new InvalidOperationException("No puedes editar una publicación de otro usuario.");

            if (string.IsNullOrWhiteSpace(req.Content))
                throw new InvalidOperationException("El contenido no puede estar vacío.");

            if (req.Image != null && !string.IsNullOrWhiteSpace(req.YouTubeUrl))
                throw new InvalidOperationException("Adjunta imagen o YouTube, no ambos.");

            string? youTubeId = null;
            if (!string.IsNullOrWhiteSpace(req.YouTubeUrl))
            {
                youTubeId = NormalizeYouTubeId(req.YouTubeUrl!);
                if (string.IsNullOrWhiteSpace(youTubeId))
                    throw new InvalidOperationException("URL de YouTube inválida.");
            }

            string? newImagePath = null;
            if (req.Image != null)
                newImagePath = await _files.SavePostImageAsync(req.Image, req.MaxImageBytes);

            post.Content = req.Content.Trim();

            if (newImagePath != null)
            {
                post.ImagePath = newImagePath;
                post.YouTubeVideoId = null;
            }
            else if (!string.IsNullOrWhiteSpace(youTubeId))
            {
                post.ImagePath = null;
                post.YouTubeVideoId = youTubeId;
            }
            else
            {
            }

            await _posts.SaveChangesAsync();
        }

        public async Task DeleteAsync(DeletePostRequest req)
        {
            var post = await _posts.GetByIdAsync(req.PostId)
                       ?? throw new InvalidOperationException("Publicación no encontrada.");

            if (post.UserId != req.UserId)
                throw new InvalidOperationException("No puedes eliminar una publicación de otro usuario.");

            post.IsDeleted = true;

            await _posts.SaveChangesAsync();
        }

        public async Task<PagedResult<PostFeedItemDto>> GetFeedAsync(GetFeedRequest req)
        {
            var total = await _posts.CountAsync(req.UserIdFilter);
            var page = await _posts.GetFeedAsync(req.UserIdFilter, req.Page, req.PageSize);

            var items = new PostFeedItemDto[page.Count];
            for (int i = 0; i < page.Count; i++)
            {
                var p = page[i];
                var ub = await _users.GetBasicAsync(p.UserId);

                items[i] = new PostFeedItemDto
                {
                    Id = p.Id,
                    AuthorId = p.UserId,
                    AuthorName = ub?.FullName ?? "Usuario",
                    AuthorAvatarPath = ub?.AvatarPath,
                    Content = p.Content,
                    ImagePath = p.ImagePath,
                    YouTubeVideoId = p.YouTubeVideoId,
                    CreatedAtUtc = p.CreatedAtUtc,
                    LikeCount = p.LikeCount,
                    DislikeCount = p.DislikeCount,
                    MyReactionIsLike = await _reactions.AnyAsync(p.Id, req.CurrentUserId, ReactionType.Like),
                    MyReactionIsDislike = await _reactions.AnyAsync(p.Id, req.CurrentUserId, ReactionType.Dislike)
                };
            }

            return new PagedResult<PostFeedItemDto>
            {
                Page = req.Page,
                PageSize = req.PageSize,
                Total = total,
                Items = items
            };
        }
        public async Task<PostForEditDto> GetForEditAsync(Guid postId, string userId)
        {
            var post = await _posts.GetByIdAsync(postId) ?? throw new InvalidOperationException("Publicación no encontrada.");
            if (post.IsDeleted) throw new InvalidOperationException("Publicación eliminada.");
            if (post.UserId != userId) throw new InvalidOperationException("No puedes editar esta publicación.");

            return new PostForEditDto
            {
                Id = post.Id,
                Content = post.Content,
                ImagePath = post.ImagePath,
                YouTubeVideoId = post.YouTubeVideoId,
                IsMine = true
            };
        }
        public async Task<ToggleReactionResult> ToggleReactionAsync(ToggleReactionRequest req)
        {
            var post = await _posts.GetByIdAsync(req.PostId)
                       ?? throw new InvalidOperationException("Publicación no encontrada.");

            var existing = await _reactions.GetAsync(req.PostId, req.UserId);
            string state;

            if (existing == null)
            {
                await _reactions.AddAsync(new Reaction
                {
                    Id = Guid.NewGuid(),
                    PostId = req.PostId,
                    UserId = req.UserId,
                    Type = req.ReactionType,
                    CreatedAtUtc = DateTime.UtcNow
                });

                if (req.ReactionType == ReactionType.Like) post.LikeCount++;
                else post.DislikeCount++;

                state = req.ReactionType == ReactionType.Like ? "liked" : "disliked";
            }
            else if (existing.Type == req.ReactionType)
            {
                if (existing.Type == ReactionType.Like) post.LikeCount--;
                else post.DislikeCount--;

                await _reactions.RemoveAsync(existing);
                state = "none";
            }
            else
            {
                if (existing.Type == ReactionType.Like)
                {
                    post.LikeCount--;
                    post.DislikeCount++;
                }
                else
                {
                    post.DislikeCount--;
                    post.LikeCount++;
                }
                existing.Type = req.ReactionType;
                state = req.ReactionType == ReactionType.Like ? "liked" : "disliked";
            }

            await _posts.SaveChangesAsync();
            await _reactions.SaveChangesAsync();

            return new ToggleReactionResult
            {
                LikeCount = post.LikeCount,
                DislikeCount = post.DislikeCount,
                State = state
            };
        }

        private static string? NormalizeYouTubeId(string url)
        {
            var patterns = new[]
            {
                @"youtu\.be/(?<id>[A-Za-z0-9_-]{6,})",
                @"youtube\.com/watch\?v=(?<id>[A-Za-z0-9_-]{6,})",
                @"youtube\.com/shorts/(?<id>[A-Za-z0-9_-]{6,})"
            };
            foreach (var p in patterns)
            {
                var m = Regex.Match(url, p, RegexOptions.IgnoreCase);
                if (m.Success) return m.Groups["id"].Value;
            }
            return null;
        }

    }
}
