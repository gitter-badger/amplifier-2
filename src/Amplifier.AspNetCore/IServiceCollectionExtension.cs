using Amplifier.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Amplifier.AspNetCore
{
    /// <summary>
    /// IServiceCollection extension methods.
    /// </summary>
    public static class IServiceCollectionExtension
    {
        /// <summary>
        /// Integrates Amplifier to Asp.NET Core
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAmplifier(this IServiceCollection services)
        {
            services.AddScoped(typeof(IUserSession<>), typeof(UserSession<>));
            return services;
        }
    }
}
