using LinkUp.Shared.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkUp.Infrastructure.Shared.Mail
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddSharedLayerIoc(this IServiceCollection services, IConfiguration cfg)
        {
            services.Configure<MailSettings>(cfg.GetSection("MailSettings"));
            services.AddTransient<IEmailSender, MailKitEmailSender>();
            services.AddHttpContextAccessor();
            return services;
        }
    }
}
