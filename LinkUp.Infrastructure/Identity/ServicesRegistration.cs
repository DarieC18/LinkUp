using LinkUp.Infrastructure.Identity.Contexts;
using LinkUp.Infrastructure.Identity.Entities;
using LinkUp.Infrastructure.Identity.Services;
using LinkUp.Infrastructure.Shared.Mail;
using LinkUp.Shared.Mail;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LinkUp.Infrastructure.Identity
{
    public static class ServicesRegistration
    {
        public static IServiceCollection AddIdentityLayerIocForWebApp(this IServiceCollection services, IConfiguration cfg)
        {
            services.AddDbContext<IdentityContext>(opt =>
                opt.UseSqlServer(
                    cfg.GetConnectionString("Default"),
                    b => b.MigrationsAssembly(typeof(IdentityContext).Assembly.GetName().Name)));

            services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.SignIn.RequireConfirmedAccount = true;
                opt.Password.RequiredLength = 6;
                opt.Lockout.MaxFailedAccessAttempts = 5;
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            })
            .AddEntityFrameworkStores<IdentityContext>()
            .AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(o => o.TokenLifespan = TimeSpan.FromHours(2));

            services.ConfigureApplicationCookie(o =>
            {
                o.LoginPath = "/Account/Login";
                o.AccessDeniedPath = "/Account/Denied";
                o.SlidingExpiration = true;
                o.Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = ctx =>
                    {
                        var target = QueryHelpers.AddQueryString(ctx.RedirectUri, "msg", "auth");
                        ctx.Response.Redirect(target);
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddHttpContextAccessor();

            services.Configure<MailSettings>(cfg.GetSection("Mail"));
            services.AddScoped<IEmailSender, MailKitEmailSender>();

            services.AddScoped<AccountServiceForWebApp>();

            return services;
        }
    }
}
