using LinkUp.Domain.Entities.Battleship;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Infrastructure.Persistence.Configurations.Battleship
{
    public class BattleshipBoardConfiguration : IEntityTypeConfiguration<BattleshipBoard>
    {
        public void Configure(EntityTypeBuilder<BattleshipBoard> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OwnerUserId)
                   .HasMaxLength(450)
                   .IsRequired();

            builder.Property(x => x.CellsCompressed)
                   .HasColumnType("nvarchar(max)");

            builder.HasIndex(x => new { x.GameId, x.OwnerUserId })
                   .IsUnique();

            builder.HasMany(x => x.ShipPlacements)
                   .WithOne(x => x.Board)
                   .HasForeignKey(x => x.BoardId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Navigation(x => x.ShipPlacements).AutoInclude();
        }
    }
}
