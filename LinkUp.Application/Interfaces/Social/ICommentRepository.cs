using LinkUp.Domain.Entities.Social;

namespace LinkUp.Application.Interfaces.Social
{
    public interface ICommentRepository
    {
        Task AddAsync(Comment comment);
        Task<Comment?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Comment>> GetForPostAsync(Guid postId);
        Task SaveChangesAsync();
    }
}
