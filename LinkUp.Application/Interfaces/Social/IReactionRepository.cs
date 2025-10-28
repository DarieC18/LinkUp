using LinkUp.Domain.Entities.Social;
using LinkUp.Domain.Enums;

namespace LinkUp.Application.Interfaces.Social
{
    public interface IReactionRepository
    {
        Task<Reaction?> GetAsync(Guid postId, string userId);
        Task AddAsync(Reaction reaction);
        Task RemoveAsync(Reaction reaction);
        Task<bool> AnyAsync(Guid postId, string userId, ReactionType type);
        Task SaveChangesAsync();
    }
}
