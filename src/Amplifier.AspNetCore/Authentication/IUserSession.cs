using System.Collections.Generic;

namespace Amplifier.AspNetCore.Authentication
{
    /// <summary>
    /// Interface that represents an user session.
    /// </summary>
    /// <typeparam name="TKey">Nullable Tenant primary key type.</typeparam>
    public interface IUserSession<TKey>
    {
        /// <summary>
        /// Unique User identifier.
        /// </summary>
        string UserId { get; set; }

        /// <summary>
        /// Unique Tenant identifier.
        /// </summary>
        TKey TenantId { get; set; }

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
