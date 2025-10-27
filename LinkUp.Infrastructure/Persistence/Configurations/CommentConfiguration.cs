using LinkUp.Domain.Entities.Social;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Infrastructure.Persistence.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comments");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.UserId)
                   .IsRequired()
                   .HasMaxLength(450);

            builder.Property(c => c.Content)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(c => c.CreatedAtUtc).IsRequired();

            builder.HasIndex(c => new { c.PostId, c.CreatedAtUtc });
            builder.HasIndex(c => c.ParentCommentId);

            builder.HasOne(c => c.ParentComment)
                   .WithMany()
                   .HasForeignKey(c => c.ParentCommentId)
                   .OnDelete(DeleteBehavior.NoAction); // evitar cascadas en cadena

        }
    }
}
