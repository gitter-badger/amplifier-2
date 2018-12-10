using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Amplifier.AspNetCore.Authentication
{
    /// <summary>
    /// Aplication builder extension methods.
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Middleware to recover user token in every request header.
        /// </summary>        
        /// <typeparam name="TKey">User primary key type</typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseUserSession<TKey>(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserSessioMiddleware<TKey>>();
        }

        /// <summary>
        /// Middleware to recover user token in every request header.
        /// </summary>        
        /// /// <typeparam name="TKey">User primary key type</typeparam>
        public class UserSessioMiddleware<TKey>
        {
            private readonly RequestDelegate _next;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="next"></param>
            public UserSessioMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="context"></param>
            /// <param name="session"></param>
            /// <returns></returns>
            public async Task InvokeAsync(HttpContext context, IUserSession<TKey> session)
            {
                if (context.User.Identities.Any(id => id.IsAuthenticated))
                {
                    session.UserId = ConvertTo<TKey>(context.User.Claims.FirstOrDefault(x => x.Type == "userid").Value);
                    session.TenantId = ConvertTo<int?>(context.User.Claims.FirstOrDefault(x => x.Type == "tenantid").Value);
                    session.Roles = context.User.Claims.Where(x => x.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(x => x.Value).ToList();
                    session.UserName = context.User.Claims.FirstOrDefault(x => x.Type == "username").Value;
                }

                await _next.Invoke(context);
            }
        }

        private static T ConvertTo<T>(this object value)
        {
            if (value is T variable) return variable;

            try
            {                
                if (Nullable.GetUnderlyingType(typeof(T)) != null)
                {
                    return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(value);
                }

                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }
}
