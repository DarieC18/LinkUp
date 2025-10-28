using LinkUp.Application.Interfaces.Social;
using LinkUp.Domain.Entities.Social;
using LinkUp.Domain.Enums;
using LinkUp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


public class ReactionRepository : IReactionRepository
{
    private readonly ApplicationDbContext _db;
    public ReactionRepository(ApplicationDbContext db) => _db = db;

    public Task<Reaction?> GetAsync(Guid postId, string userId) =>
        _db.Reactions.FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId)!;
    public Task AddAsync(Reaction r) { _db.Reactions.Add(r); return Task.CompletedTask; }
    public Task RemoveAsync(Reaction r) { _db.Reactions.Remove(r); return Task.CompletedTask; }
    public Task<bool> AnyAsync(Guid postId, string userId, ReactionType type) =>
        _db.Reactions.AnyAsync(r => r.PostId == postId && r.UserId == userId && r.Type == type);
    public Task SaveChangesAsync() => _db.SaveChangesAsync();
}