using LinkUp.Domain.Entities.Battleship;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Infrastructure.Persistence.Configurations.Battleship
{
    public class BattleshipGameConfiguration : IEntityTypeConfiguration<BattleshipGame>
    {
        public void Configure(EntityTypeBuilder<BattleshipGame> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status)
                   .IsRequired();

            builder.Property(x => x.Player1Id)
                   .HasMaxLength(450)
                   .IsRequired();

            builder.Property(x => x.Player2Id)
                   .HasMaxLength(450)
                   .IsRequired();

            builder.HasIndex(x => new { x.Player1Id, x.Player2Id, x.Status });

            builder.HasMany(x => x.Boards)
                   .WithOne(x => x.Game)
                   .HasForeignKey(x => x.GameId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Attacks)
                   .WithOne(x => x.Game)
                   .HasForeignKey(x => x.GameId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
