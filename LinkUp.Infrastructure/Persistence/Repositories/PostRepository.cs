using LinkUp.Application.Interfaces.Social;
using LinkUp.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;


namespace LinkUp.Infrastructure.Persistence.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _db;
        public PostRepository(ApplicationDbContext db) => _db = db;

        public async Task<Guid> AddAsync(Post post) { _db.Posts.Add(post); await _db.SaveChangesAsync(); return post.Id; }
        public Task<Post?> GetByIdAsync(Guid id) => _db.Posts.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted)!;
        public async Task<IReadOnlyList<Post>> GetFeedAsync(string? userId, int page, int size) =>
            await _db.Posts.Where(p => !p.IsDeleted && (userId == null || p.UserId == userId))
                           .OrderByDescending(p => p.CreatedAtUtc)
                           .Skip((page - 1) * size).Take(size).ToListAsync();
        public Task<int> CountAsync(string? userId) =>
            _db.Posts.CountAsync(p => !p.IsDeleted && (userId == null || p.UserId == userId));
        public Task SaveChangesAsync() => _db.SaveChangesAsync();
    }
}
