using LinkUp.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Infrastructure.Persistence.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.UserId)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.Property(p => p.Content)
                   .HasMaxLength(2000);

            builder.Property(p => p.ImagePath)
                   .HasMaxLength(1024);

            builder.Property(p => p.YouTubeVideoId)
                   .HasMaxLength(32);

            builder.Property(p => p.CreatedAtUtc).IsRequired();
            builder.Property(p => p.IsDeleted).HasDefaultValue(false);

            builder.Property(p => p.LikeCount).HasDefaultValue(0);
            builder.Property(p => p.DislikeCount).HasDefaultValue(0);

            builder.HasIndex(p => p.CreatedAtUtc);
            builder.HasIndex(p => new { p.UserId, p.CreatedAtUtc });

            builder.HasMany(p => p.Comments)
                   .WithOne(c => c.Post)
                   .HasForeignKey(c => c.PostId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Reactions)
                   .WithOne(r => r.Post)
                   .HasForeignKey(r => r.PostId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
