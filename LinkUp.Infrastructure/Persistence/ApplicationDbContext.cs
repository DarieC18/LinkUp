using LinkUp.Domain.Entities.Battleship;
using LinkUp.Domain.Entities.Social;
using LinkUp.Infrastructure.Persistence.Configurations.Battleship;
using Microsoft.EntityFrameworkCore;

namespace LinkUp.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Reaction> Reactions => Set<Reaction>();
        public DbSet<FriendRequest> FriendRequests => Set<FriendRequest>();
        public DbSet<Friendship> Friendships => Set<Friendship>();

        // Battleship
        public DbSet<BattleshipGame> BattleshipGames => Set<BattleshipGame>();
        public DbSet<BattleshipBoard> BattleshipBoards => Set<BattleshipBoard>();
        public DbSet<BattleshipShipPlacement> BattleshipShipPlacements => Set<BattleshipShipPlacement>();
        public DbSet<BattleshipAttack> BattleshipAttacks => Set<BattleshipAttack>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            // Battleship
            modelBuilder.ApplyConfiguration(new BattleshipGameConfiguration());
            modelBuilder.ApplyConfiguration(new BattleshipBoardConfiguration());
            modelBuilder.ApplyConfiguration(new BattleshipShipPlacementConfiguration());
            modelBuilder.ApplyConfiguration(new BattleshipAttackConfiguration());



        }

    }
}
