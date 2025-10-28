using LinkUp.Application.DTOs.Social;
using LinkUp.Application.Interfaces.Social;
using LinkUp.Application.Interfaces.Users;
using LinkUp.Domain.Entities.Social;

namespace LinkUp.Application.Services.Social
{
    public sealed class CommentService : ICommentService
    {
        private readonly ICommentRepository _comments;
        private readonly IUsersReadOnly _users;

        public CommentService(ICommentRepository comments, IUsersReadOnly users)
        {
            _comments = comments;
            _users = users;
        }

        public async Task<IReadOnlyList<CommentDto>> GetThreadAsync(Guid postId, string currentUserId)
        {
            var list = await _comments.GetForPostAsync(postId);
            var map = new Dictionary<Guid, CommentDto>();
            var roots = new List<CommentDto>();

            foreach (var c in list.OrderBy(x => x.CreatedAtUtc))
            {
                var ub = await _users.GetBasicAsync(c.UserId);
                var dto = new CommentDto
                {
                    Id = c.Id,
                    PostId = c.PostId,
                    UserId = c.UserId,
                    ParentCommentId = c.ParentCommentId,
                    Content = c.Content,
                    CreatedAtUtc = c.CreatedAtUtc,
                    AuthorName = ub?.FullName ?? "Usuario",
                    AuthorAvatarPath = ub?.AvatarPath,
                    IsMine = (c.UserId == currentUserId)
                };
                map[c.Id] = dto;

                if (c.ParentCommentId == null) roots.Add(dto);
                else if (map.TryGetValue(c.ParentCommentId.Value, out var parent))
                    parent.Replies.Add(dto);
                else
                    roots.Add(dto);
            }

            return roots;
        }

        public async Task<Guid> AddCommentAsync(CreateCommentRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Content))
                throw new InvalidOperationException("El comentario no puede estar vacío.");

            var c = new Comment
            {
                Id = Guid.NewGuid(),
                PostId = req.PostId,
                UserId = req.UserId,
                ParentCommentId = null,
                Content = req.Content.Trim(),
                CreatedAtUtc = DateTime.UtcNow,
                IsDeleted = false
            };
            await _comments.AddAsync(c);
            await _comments.SaveChangesAsync();
            return c.Id;
        }

        public async Task<Guid> AddReplyAsync(CreateReplyRequest req)
        {
            if (string.IsNullOrWhiteSpace(req.Content))
                throw new InvalidOperationException("El reply no puede estar vacío.");

            var c = new Comment
            {
                Id = Guid.NewGuid(),
                PostId = req.PostId,
                UserId = req.UserId,
                ParentCommentId = req.ParentCommentId,
                Content = req.Content.Trim(),
                CreatedAtUtc = DateTime.UtcNow,
                IsDeleted = false
            };
            await _comments.AddAsync(c);
            await _comments.SaveChangesAsync();
            return c.Id;
        }

        public async Task EditAsync(EditCommentRequest req)
        {
            var c = await _comments.GetByIdAsync(req.CommentId) ?? throw new InvalidOperationException("Comentario no encontrado.");
            if (c.UserId != req.UserId) throw new InvalidOperationException("No puedes editar comentarios de otro usuario.");
            if (string.IsNullOrWhiteSpace(req.Content)) throw new InvalidOperationException("El comentario no puede estar vacío.");

            c.Content = req.Content.Trim();
            await _comments.SaveChangesAsync();
        }

        public async Task DeleteAsync(DeleteCommentRequest req)
        {
            var c = await _comments.GetByIdAsync(req.CommentId) ?? throw new InvalidOperationException("Comentario no encontrado.");
            if (c.UserId != req.UserId) throw new InvalidOperationException("No puedes borrar comentarios de otro usuario.");

            c.IsDeleted = true;
            c.Content = "[eliminado]";
            await _comments.SaveChangesAsync();
        }
    }
}
