using LinkUp.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Infrastructure.Persistence.Configurations
{
    public class ReactionConfiguration : IEntityTypeConfiguration<Reaction>
    {
        public void Configure(EntityTypeBuilder<Reaction> builder)
        {
            builder.ToTable("Reactions");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.UserId)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.Property(r => r.Type)
                   .IsRequired();

            builder.Property(r => r.CreatedAtUtc)
                   .IsRequired();

            // Un usuario solo puede reaccionar una vez a un post
            builder.HasIndex(r => new { r.PostId, r.UserId })
                   .IsUnique();
        }
    }
}
