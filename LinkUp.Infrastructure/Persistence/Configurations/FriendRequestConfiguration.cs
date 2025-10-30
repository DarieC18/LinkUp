using LinkUp.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Infrastructure.Persistence.Configurations
{
    public class FriendRequestConfiguration : IEntityTypeConfiguration<FriendRequest>
    {
        public void Configure(EntityTypeBuilder<FriendRequest> b)
        {
            b.ToTable("FriendRequests");
            b.HasKey(x => x.Id);

            b.Property(x => x.FromUserId).IsRequired().HasMaxLength(450);
            b.Property(x => x.ToUserId).IsRequired().HasMaxLength(450);

            b.Property(x => x.Status).IsRequired();

            b.Property(x => x.CreatedAtUtc)
             .HasColumnType("datetime2")
             .HasDefaultValueSql("GETUTCDATE()");

            b.Property(x => x.RespondedAtUtc)
             .HasColumnType("datetime2");

            b.HasIndex(x => new { x.ToUserId, x.Status });

            b.HasIndex(x => new { x.FromUserId, x.ToUserId })
             .IsUnique()
             .HasFilter("[Status] = 0");

        }
    }
}
