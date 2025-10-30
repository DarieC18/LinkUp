using LinkUp.Domain.Entities.Battleship;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LinkUp.Infrastructure.Persistence.Configurations.Battleship
{
    public class BattleshipAttackConfiguration : IEntityTypeConfiguration<BattleshipAttack>
    {
        public void Configure(EntityTypeBuilder<BattleshipAttack> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AttackerUserId)
                   .HasMaxLength(450)
                   .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                   .IsRequired();

            builder.HasIndex(x => new { x.GameId, x.AttackerUserId, x.Row, x.Col })
                   .IsUnique();

            builder.Property(x => x.Row).IsRequired();
            builder.Property(x => x.Col).IsRequired();
        }
    }
}
