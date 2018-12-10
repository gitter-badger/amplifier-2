using System.Collections.Generic;

namespace Amplifier.AspNetCore.Authentication
{
    /// <summary>
    /// Class that implements an user session.
    /// </summary>   
    /// <typeparam name="TKey">User primary key type.</typeparam>
    public class UserSession<TKey> : IUserSession<TKey>
    {
        /// <summary>
        /// Unique User identifier.
        /// </summary>
        public TKey UserId { get; set; }

        /// <summary>
        /// Unique Tenant identifier.
        /// </summary>
        public int? TenantId { get; set; }

        /// <summary>
        /// List of user roles names.
        /// </summary>
        public List<string> Roles { get; set; }

        /// <summary>
        /// User name.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Disable Tenant automatic filter.
        /// </summary>
        public bool DisableTenantFilter { get; set; }
    }
}
