using Api.Authentication;
using Api.Data;
using Api.Data.Entities.Account;
using Api.Services.Auth;
using Api.Services.Infrastructure;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Api
{
    public static class AppBootstrapper
    {
        /// <summary>
        /// Registers All Services In the core project into container
        /// </summary>
        /// <param name="services"></param>
        public static void InitCoreServicesAndRepositories(this IServiceCollection services)
        {
            services.AddTransient<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IOtpGenerator, OtpGenerator>();
            services.AddTransient<ISessionService, SessionService>();            
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddHttpClient();
            AutoInjectLayers(ref services);
        }

        private static void AutoInjectLayers(ref IServiceCollection serviceCollection)
        {
            serviceCollection.Scan(scan => scan.FromCallingAssembly().AddClasses(classes => classes
                    .Where(type => type.Name.EndsWith("Repository") || type.Name.EndsWith("Service")), false)
                .AsImplementedInterfaces()
                .WithScopedLifetime());
        }
    }
}