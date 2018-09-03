using Amplifier.AspNetCore.Entities;
using System.ComponentModel.DataAnnotations;

namespace Amplifier.AspNetCore.MultiTenancy
{
    /// <summary>
    /// Tenant base class.
    /// </summary>
    /// <typeparam name="TKey">Primary key of Tenant entity</typeparam>
    public class TenantBase<TKey> : IEntity<TKey>
    {
        /// <summary>
        /// Max length of the <see cref="Name"/> property.
        /// </summary>
        public const int TenantNameMaxLength = 64;

        /// <summary>
        /// Min length of the <see cref="Name"/> property.
        /// </summary>
        public const int TenantNameMinLength = 3;

        /// <summary>
        /// Max length of the <see cref="DisplayName"/> property.
        /// </summary>
        public const int TenantDisplayNameMaxLength = 128;

        /// <summary>
        /// Min length of the <see cref="DisplayName"/> property.
        /// </summary>
        public const int TenantDisplayNameMinLength = 3;

        /// <summary>
        /// Regex of the <see cref="Name"/> property.
        /// </summary>
        public const string TenantNameRegex = "^[a-zA-Z][a-zA-Z0-9_-]{1,}$";

        /// <summary>
        /// Tenant unique identifier.
        /// </summary>
        public TKey Id { get; set; }

        /// <summary>
        /// Tenant unique name.
        /// </summary>
        [Required]
        [MaxLength(TenantNameMaxLength)]
        [MinLength(TenantNameMinLength)]
        public string Name { get; set; }

        /// <summary>
        /// Tenant display name.
        /// </summary>
        [Required]
        [MaxLength(TenantDisplayNameMaxLength)]
        [MinLength(TenantDisplayNameMinLength)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Used to allow/block tenant access to the application. 
        /// </summary>
        public bool IsActive { get; set; }
    }
}
