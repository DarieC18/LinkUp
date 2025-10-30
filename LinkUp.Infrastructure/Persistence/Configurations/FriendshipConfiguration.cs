using LinkUp.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Infrastructure.Persistence.Configurations
{
    public class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
    {
        public void Configure(EntityTypeBuilder<Friendship> b)
        {
            b.ToTable("Friendships");

            b.HasKey(x => new { x.UserId1, x.UserId2 });

            b.Property(x => x.UserId1).IsRequired().HasMaxLength(450);
            b.Property(x => x.UserId2).IsRequired().HasMaxLength(450);

            b.Property(x => x.CreatedAtUtc)
             .HasColumnType("datetime2")
             .HasDefaultValueSql("GETUTCDATE()");

            b.HasIndex(x => x.UserId1);
            b.HasIndex(x => x.UserId2);
        }
    }
}
