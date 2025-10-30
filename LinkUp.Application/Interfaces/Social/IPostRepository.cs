using LinkUp.Domain.Entities.Social;

namespace LinkUp.Application.Interfaces.Social
{
    public interface IPostRepository
    {
        Task<Guid> AddAsync(Post post);
        Task<Post?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Post>> GetFeedAsync(string? userIdFilter, int page, int pageSize);
        Task<int> CountAsync(string? userIdFilter);
        Task SaveChangesAsync();
        Task<IReadOnlyList<Post>> GetFeedByAuthorsAsync(IReadOnlyCollection<string> authorIds, int page, int pageSize);
        Task<int> CountByAuthorsAsync(IReadOnlyCollection<string> authorIds);

    }
}
