using LinkUp.Domain.Entities.Battleship;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Infrastructure.Persistence.Configurations.Battleship
{
    public class BattleshipShipPlacementConfiguration : IEntityTypeConfiguration<BattleshipShipPlacement>
    {
        public void Configure(EntityTypeBuilder<BattleshipShipPlacement> builder)
        {
            builder.ToTable("BattleshipShipPlacements");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                   .ValueGeneratedNever();

            builder.Property(x => x.BoardId)
                   .IsRequired();

            builder.HasOne(x => x.Board)
                   .WithMany(b => b.ShipPlacements)
                   .HasForeignKey(x => x.BoardId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.OriginRow).IsRequired();
            builder.Property(x => x.OriginCol).IsRequired();

            builder.Property(x => x.Direction)
                   .HasConversion<int>()
                   .IsRequired();

            builder.Property(x => x.ShipType)
                   .HasConversion<int>()
                   .IsRequired();

            builder.HasIndex(x => new { x.BoardId, x.ShipType })
                   .IsUnique();
        }
    }
}
