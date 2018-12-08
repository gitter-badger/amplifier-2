using System.Collections.Generic;

namespace Amplifier.AspNetCore.Authentication
{
    /// <summary>
    /// Interface that represents an user session.
    /// </summary>
    /// <typeparam name="TTenantKey">Tenant primary key type.</typeparam>
    /// <typeparam name="TUserKey">User primary key type.</typeparam>
    public interface IUserSession<TTenantKey, TUserKey>
    {
        /// <summary>
        /// Unique User identifier.
        /// </summary>
        TUserKey UserId { get; set; }

        /// <summary>
        /// Unique Tenant identifier.
        /// </summary>
        TTenantKey TenantId { get; set; }

        /// <summary>
        /// List of user roles names.
        /// </summary>
        List<string> Roles { get; set; }

        /// <summary>
        /// User name.
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// Disable Tenant automatic filter.
        /// </summary>
        bool DisableTenantFilter { get; set; }
    }
}
