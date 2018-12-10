using System.Collections.Generic;

namespace Amplifier.AspNetCore.Authentication
{
    /// <summary>
    /// Interface that represents an user session.
    /// </summary>    
    /// <typeparam name="TKey">User primary key type.</typeparam>
    public interface IUserSession<TKey>
    {
        /// <summary>
        /// Unique User identifier.
        /// </summary>
        TKey UserId { get; set; }

        /// <summary>
        /// Unique Tenant identifier.
        /// </summary>
        int? TenantId { get; set; }

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
