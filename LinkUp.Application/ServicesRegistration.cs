using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LinkUp.Application
{
    public static class ServicesRegistration
    {
        public static void AddApplicationLayerIoc(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

        }
    }
}
