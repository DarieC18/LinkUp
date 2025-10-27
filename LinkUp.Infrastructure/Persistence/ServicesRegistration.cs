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

            return services;
        }
    }
}
