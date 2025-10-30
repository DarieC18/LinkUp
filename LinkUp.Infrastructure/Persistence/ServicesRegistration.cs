using LinkUp.Application.Common.Persistence;
using LinkUp.Application.Interfaces.Battleship;
using LinkUp.Application.Interfaces.Social;
using LinkUp.Application.Interfaces.Storage;
using LinkUp.Application.Interfaces.Users;
using LinkUp.Application.Services.Social;
using LinkUp.Infrastructure.Common;
using LinkUp.Infrastructure.Identity.Queries;
using LinkUp.Infrastructure.Persistence.Repositories;
using LinkUp.Infrastructure.Persistence.Repositories.Battleship;
using LinkUp.Infrastructure.Repositories;
using LinkUp.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkUp.Infrastructure.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistenceLayerIoc(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddDbContext<ApplicationDbContext>(
                opt =>
                {
                    opt.UseSqlServer(cfg.GetConnectionString("Default"));
                }
            );

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IReactionRepository, ReactionRepository>();
            services.AddScoped<IUsersReadOnly, UsersReadOnly>();
            services.AddScoped<IFileStorage, WebRootFileStorage>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IFriendRequestRepository, FriendRequestRepository>();
            services.AddScoped<IFriendshipRepository, FriendshipRepository>();
            services.AddScoped<IFriendsService, FriendsService>();
            services.AddSingleton<IDateTime, SystemClock>();


            // Battleship
            services.AddScoped<IBattleshipGameRepository, BattleshipGameRepository>();
            services.AddScoped<IBattleshipBoardRepository, BattleshipBoardRepository>();
            services.AddScoped<IBattleshipAttackRepository, BattleshipAttackRepository>();



            return services;
        }
    }
}
