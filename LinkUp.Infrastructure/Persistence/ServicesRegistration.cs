using LinkUp.Application.Interfaces.Social;
using LinkUp.Application.Interfaces.Storage;
using LinkUp.Application.Interfaces.Users;
using LinkUp.Infrastructure.Identity.Queries;
using LinkUp.Infrastructure.Persistence.Repositories;
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
            services.AddDbContext<ApplicationDbContext>(opt =>
                opt.UseSqlServer(cfg.GetConnectionString("Default")));

            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IReactionRepository, ReactionRepository>();
            services.AddScoped<IUsersReadOnly, UsersReadOnly>();
            services.AddScoped<IFileStorage, WebRootFileStorage>();
            services.AddScoped<ICommentRepository, CommentRepository>();

            return services;
        }
    }
}
