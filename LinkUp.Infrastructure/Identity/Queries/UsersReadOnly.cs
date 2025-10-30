using LinkUp.Application.Interfaces.Users;
using LinkUp.Infrastructure.Identity.Contexts;
using LinkUp.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Identity.Queries
{
    public sealed class UsersReadOnly : IUsersReadOnly
    {
        private readonly IdentityContext _db;
        public UsersReadOnly(IdentityContext db) => _db = db;

        public async Task<UserBasicDto?> GetBasicAsync(string userId, CancellationToken ct = default)
        {
            return await _db.Set<AppUser>()
                .Where(u => u.Id == userId)
                .Select(u => new UserBasicDto
                {
                    FullName = ((u.FirstName ?? "") + " " + (u.LastName ?? "")).Trim(),
                    AvatarPath = u.ProfilePhotoPath
                })
                .FirstOrDefaultAsync(ct);
        }
        public async Task<List<UserBasicRowDto>> GetAllBasicAsync(CancellationToken ct = default)
        {
            return await _db.Set<AppUser>()
                .Select(u => new UserBasicRowDto
                {
                    Id = u.Id,
                    FullName = ((u.FirstName ?? "") + " " + (u.LastName ?? "")).Trim(),
                    AvatarPath = u.ProfilePhotoPath
                })
                .OrderBy(u => u.FullName)
                .ToListAsync(ct);
        }
        public async Task<List<UserBasicRowDto>> SearchBasicAsync(string term, CancellationToken ct = default)
        {
            term = term?.Trim() ?? "";
            return await _db.Set<AppUser>()
                .Where(u =>
                    (u.FirstName + " " + u.LastName).Contains(term) ||
                    u.UserName!.Contains(term))
                .Select(u => new UserBasicRowDto
                {
                    Id = u.Id,
                    FullName = ((u.FirstName ?? "") + " " + (u.LastName ?? "")).Trim(),
                    AvatarPath = u.ProfilePhotoPath
                })
                .OrderBy(u => u.FullName)
                .ToListAsync(ct);
        }

        public async Task<string?> GetIdByUserNameAsync(string username, CancellationToken ct = default)
        {
            return await _db.Users
                .Where(u => u.UserName == username)
                .Select(u => u.Id)
                .FirstOrDefaultAsync(ct);
        }

    }
}
