using LinkUp.Application.Interfaces.Social;
using LinkUp.Domain.Entities.Social;
using LinkUp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Repositories
{
    public sealed class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _db;
        public CommentRepository(ApplicationDbContext db) => _db = db;

        public Task<Comment?> GetByIdAsync(Guid id)
            => _db.Comments.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

        public async Task AddAsync(Comment comment)
        {
            await _db.Comments.AddAsync(comment);
        }

        public async Task<IReadOnlyList<Comment>> GetForPostAsync(Guid postId)
        {
            return await _db.Comments
                .AsNoTracking()
                .Where(c => c.PostId == postId && !c.IsDeleted)
                .OrderBy(c => c.CreatedAtUtc)
                .ToListAsync();
        }

        public Task SaveChangesAsync() => _db.SaveChangesAsync();
    }
}
