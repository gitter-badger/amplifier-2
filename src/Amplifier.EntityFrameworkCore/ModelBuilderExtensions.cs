using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Amplifier.AspNetCore.MultiTenancy
{
    /// <summary>
    /// Extension method for <see cref="ModelBuilder"/>
    /// </summary>
    public static class ModelBuilderExtensions
    {
        /// <summary>
        /// Add TenantId shadow property to entities that implements IHaveTenant and IMayHaveTenant interfaces
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <typeparam name="TEntity">Tenant entity</typeparam>
        /// <param name="entitiesList">A list with all entities</param>        
        public static void MultiTenancy<TEntity>(this ModelBuilder modelBuilder, List<IMutableEntityType> entitiesList) where TEntity : class
        {
            foreach (var entityType in entitiesList)
            {
                var type = entityType.ClrType;

                if (typeof(IHaveTenant).IsAssignableFrom(type))
                {
                    var method = SetTenantShadowPropertyMethodInfo.MakeGenericMethod(type, typeof(TEntity));
                    method.Invoke(modelBuilder, new object[] { modelBuilder });
                }

                if (typeof(IMayHaveTenant).IsAssignableFrom(type))
                {
                    var method = SetNullableTenantShadowPropertyMethodInfo.MakeGenericMethod(type, typeof(TEntity));
                    method.Invoke(modelBuilder, new object[] { modelBuilder });
                }
            }
        }

        private static readonly MethodInfo SetTenantShadowPropertyMethodInfo = typeof(ModelBuilderExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(t => t.IsGenericMethod && t.Name == "SetTenantShadowProperty");

        private static readonly MethodInfo SetNullableTenantShadowPropertyMethodInfo = typeof(ModelBuilderExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(t => t.IsGenericMethod && t.Name == "SetNullableTenantShadowProperty");

        private static void SetTenantShadowProperty<T, TEntity>(ModelBuilder builder) where T : class, IHaveTenant where TEntity : class
        {
            builder.Entity<T>().Property<int>("TenantId");
            builder.Entity<T>().HasOne<TEntity>().WithMany().HasForeignKey("TenantId").OnDelete(DeleteBehavior.Restrict);
        }

        private static void SetNullableTenantShadowProperty<T, TEntity>(ModelBuilder builder) where T : class, IMayHaveTenant where TEntity : class
        {
            builder.Entity<T>().Property<int?>("TenantId");
            builder.Entity<T>().HasOne<TEntity>().WithMany().HasForeignKey("TenantId").OnDelete(DeleteBehavior.Restrict);
        }
    }
}
