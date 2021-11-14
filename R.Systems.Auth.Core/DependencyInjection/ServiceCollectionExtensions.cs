using Microsoft.Extensions.DependencyInjection;
using R.Systems.Auth.Core.Services;
using R.Systems.Auth.Core.Interfaces;

namespace R.Systems.Auth.Core.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddScoped<AuthenticationService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
        }
    }
}
