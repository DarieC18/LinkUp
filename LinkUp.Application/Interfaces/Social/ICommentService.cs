using LinkUp.Application.DTOs.Social;

namespace LinkUp.Application.Interfaces.Social
{
    public interface ICommentService
    {
        Task<IReadOnlyList<CommentDto>> GetThreadAsync(Guid postId, string currentUserId);
        Task<Guid> AddCommentAsync(CreateCommentRequest req);
        Task<Guid> AddReplyAsync(CreateReplyRequest req);
        Task EditAsync(EditCommentRequest req);
        Task DeleteAsync(DeleteCommentRequest req);
    }
}
