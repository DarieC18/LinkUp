using LinkUp.Application.Interfaces.Users;
using LinkUp.Infrastructure.Identity.Contexts;
using LinkUp.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Identity.Queries
{
    public sealed class UsersReadOnly : IUsersReadOnly
    {
        private readonly IdentityContext _db;

        public UsersReadOnly(IdentityContext db)
        {
            _db = db;
        }

        public async Task<UserBasicDto?> GetBasicAsync(string userId)
        {
            return await _db.Set<AppUser>()
                .Where(u => u.Id == userId)
                .Select(u => new UserBasicDto
                {
                    FullName = ((u.FirstName ?? "") + " " + (u.LastName ?? "")).Trim(),
                    AvatarPath = u.ProfilePhotoPath
                })
                .FirstOrDefaultAsync();
        }
    }
}
