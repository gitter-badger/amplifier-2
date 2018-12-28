using Amplifier.AspNetCore.Auditing;
using Amplifier.AspNetCore.Authentication;
using Amplifier.AspNetCore.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Amplifier.EntityFrameworkCore.Identity
{
    /// <summary>
    /// Base class for the Entity Framework database context used for identity.
    /// </summary>
    /// <typeparam name="TUser">The type of user objects.</typeparam>
    /// <typeparam name="TRole">The type of role objects.</typeparam>
    /// <typeparam name="TKey">The type of the primary key for users and roles.</typeparam>
    public class IdentityDbContextBase<TTenant, TUser, TRole, TKey> :  IdentityDbContext<TUser, TRole, TKey>
        where TTenant : TenantBase
        where TUser : IdentityUser<TKey> 
        where TRole : IdentityRole<TKey> 
        where TKey : IEquatable<TKey>        
    {
        private readonly IUserSession<TKey> _userSession;

        /// <summary>
        /// DbContextBase constructor.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="userSession"></param>
        public IdentityDbContextBase(DbContextOptions options, IUserSession<TKey> userSession)
            : base(options)
        {
            _userSession = userSession;
        }

        /// <summary>
        /// Implement SaveChangesAsync to set TenantId and auditing properties before save changes.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            ChangeTracker.AutomaticTenantIdAndAuditing(_userSession);
            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Override OnModelCreating to enable multitenancy, auditing and filters.
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var entities = modelBuilder.Model.GetEntityTypes().ToList();
            modelBuilder.MultiTenancy<TTenant>(entities);
            modelBuilder.Auditing<TUser, int?>(entities);
            EnableTenantAndSoftDeleteFilters(modelBuilder, entities);
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Add QueryFilters for auditing and soft delete.
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="entities">A list with all entities</param>
        protected void EnableTenantAndSoftDeleteFilters(ModelBuilder modelBuilder,
                                                    List<IMutableEntityType> entities)
        {
            foreach (var entityType in entities)
            {
                var type = entityType.ClrType;

                if (typeof(ISoftDelete).IsAssignableFrom(type))
                {
                    if (typeof(ITenantFilter).IsAssignableFrom(type))
                    {
                        var method = SetSoftDeleteAndTenantIdFilterMethodInfo.MakeGenericMethod(type);
                        method.Invoke(this, new object[] { modelBuilder });
                    }
                    else
                    {
                        var method = SetSoftDeleteFilterMethodInfo.MakeGenericMethod(type);
                        method.Invoke(this, new object[] { modelBuilder });
                    }
                }

            }
        }

        private static readonly MethodInfo SetSoftDeleteFilterMethodInfo = typeof(IdentityDbContextBase<TTenant, TUser, TRole, TKey>).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(t => t.IsGenericMethod && t.Name == "SetSoftDeleteFilter");

        private static readonly MethodInfo SetSoftDeleteAndTenantIdFilterMethodInfo = typeof(IdentityDbContextBase<TTenant, TUser, TRole, TKey>).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(t => t.IsGenericMethod && t.Name == "SetSoftDeleteAndTenantIdFilter");

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        public void SetSoftDeleteFilter<T>(ModelBuilder builder) where T : class, ISoftDelete
        {
            builder.Entity<T>().HasQueryFilter(item => !EF.Property<bool>(item, "IsDeleted"));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="builder"></param>
        public void SetSoftDeleteAndTenantIdFilter<T>(ModelBuilder builder) where T : class, ISoftDelete, ITenantFilter
        {
            builder.Entity<T>().HasQueryFilter(
                item => !EF.Property<bool>(item, "IsDeleted") &&
                        (_userSession.DisableTenantFilter || EF.Property<int?>(item, "TenantId") == _userSession.TenantId));
        }
    }
}
