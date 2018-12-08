using Amplifier.AspNetCore.Auditing;
using Amplifier.AspNetCore.Authentication;
using Amplifier.AspNetCore.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Amplifier.EntityFrameworkCore
{
    /// <summary>
    /// Base class for all DbContext classes.
    /// </summary>
    /// <typeparam name="TTenantKey">Tenant Primary Key type</typeparam>
    /// <typeparam name="TUserKey">User Primary Key type</typeparam>
    public class DbContextBase<TTenantKey, TUserKey> : DbContext
    {
        private readonly IUserSession<TTenantKey, TUserKey> _userSession;

        /// <summary>
        /// DbContextBase constructor.
        /// </summary>
        /// <param name="options"></param>
        /// <param name="userSession"></param>
        public DbContextBase(DbContextOptions options, IUserSession<TTenantKey, TUserKey> userSession) 
            : base(options)
        {
            _userSession = userSession;
        }

        /// <summary>
        /// Override SaveChangesAsync to set TenantId and auditing properties before save changes.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            ChangeTracker.AutomaticTenantIdAndAuditing(_userSession);
            return await SaveChangesAsync(cancellationToken);
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

        private static readonly MethodInfo SetSoftDeleteFilterMethodInfo = typeof(DbContextBase<TTenantKey, TUserKey>).GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(t => t.IsGenericMethod && t.Name == "SetSoftDeleteFilter");

        private static readonly MethodInfo SetSoftDeleteAndTenantIdFilterMethodInfo = typeof(DbContextBase<TTenantKey, TUserKey>).GetMethods(BindingFlags.Public | BindingFlags.Instance)
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
                        (_userSession.DisableTenantFilter || EqualityComparer<TTenantKey>.Default.Equals(EF.Property<TTenantKey>(item, "TenantId"), _userSession.TenantId)));
        }
    }
}
