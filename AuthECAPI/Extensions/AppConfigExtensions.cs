using AuthECAPI.Models;
using Microsoft.Extensions.Options;

namespace AuthECAPI.Extensions
{
    public static class AppConfigExtensions
    {
        public static WebApplication ConfigureCors(this WebApplication app,IConfiguration configuration)
        {
            app.UseCors();
            return app;
        }

        public static IServiceCollection AddConfigForAppSettings(this IServiceCollection services,IConfiguration configuration)
        {

            services.Configure<AppSettings>(configuration.GetSection("JWT"));
            return services;
        }
    }
}
