using LinkUp.Application.Interfaces.Battleship;
using LinkUp.Application.Interfaces.Social;
using LinkUp.Application.Mapping;
using LinkUp.Application.Services.Battleship;
using LinkUp.Application.Services.Social;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkUp.Application
{
    public static class ServicesRegistration
    {
        public static void AddApplicationLayerIoc(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddAutoMapper(typeof(BattleshipProfile).Assembly);
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<IBattleshipService, BattleshipService>();

        }
    }
}
