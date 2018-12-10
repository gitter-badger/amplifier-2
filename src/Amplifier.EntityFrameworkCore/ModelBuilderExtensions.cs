using Amplifier.AspNetCore.Auditing;
using Amplifier.AspNetCore.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Amplifier.EntityFrameworkCore
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

        public static void SetTenantShadowProperty<T, TEntity>(ModelBuilder builder) where T : class, IHaveTenant where TEntity : class
        {
            builder.Entity<T>().Property<int>("TenantId");
            builder.Entity<T>().HasOne<TEntity>().WithMany().HasForeignKey("TenantId").OnDelete(DeleteBehavior.Restrict);
        }

        public static void SetNullableTenantShadowProperty<T, TEntity>(ModelBuilder builder) where T : class, IMayHaveTenant where TEntity : class
        {
            builder.Entity<T>().Property<int?>("TenantId");
            builder.Entity<T>().HasOne<TEntity>().WithMany().HasForeignKey("TenantId").OnDelete(DeleteBehavior.Restrict);
        }

        /// <summary>
        /// Add auditing shadow property to entities that implements ISoftDelete and IFullAuditedEntity interfaces
        /// </summary>
        /// <typeparam name="TUser">Entity that represents an User</typeparam>        
        /// <param name="modelBuilder"></param>
        /// <param name="entities">A list with all entities</param>
        public static void Auditing<TUser>(this ModelBuilder modelBuilder, List<IMutableEntityType> entities) where TUser : class
        {
            foreach (var entityType in entities)
            {
                var type = entityType.ClrType;

                if (typeof(ISoftDelete).IsAssignableFrom(type))
                {
                    var method = SetSoftDeleteShadowPropertyMethodInfo.MakeGenericMethod(type);
                    method.Invoke(modelBuilder, new object[] { modelBuilder });
                }

                if (typeof(IFullAuditedEntity).IsAssignableFrom(type))
                {
                    var method = SetFullAuditingShadowPropertyPropertyMethodInfo.MakeGenericMethod(type, typeof(TUser));
                    method.Invoke(modelBuilder, new object[] { modelBuilder });
                }
            }
        }

        private static readonly MethodInfo SetSoftDeleteShadowPropertyMethodInfo = typeof(ModelBuilderExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(t => t.IsGenericMethod && t.Name == "SetSoftDeleteShadowProperty");

        private static readonly MethodInfo SetFullAuditingShadowPropertyPropertyMethodInfo = typeof(ModelBuilderExtensions).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Single(t => t.IsGenericMethod && t.Name == "SetFullAuditingShadowProperty");

        public static void SetSoftDeleteShadowProperty<T>(ModelBuilder builder) where T : class, ISoftDelete
        {
            builder.Entity<T>().Property<bool>("IsDeleted");
        }

        public static void SetFullAuditingShadowProperty<T, TUser>(ModelBuilder builder) where T : class, IFullAuditedEntity where TUser : class
        {
            builder.Entity<T>().Property<DateTime>("CreationTime");
            builder.Entity<T>().Property<DateTime>("LastModificationTime");
            builder.Entity<T>().Property<DateTime>("DeletionTime");
            builder.Entity<T>().Property<string>("CreationUser");
            builder.Entity<T>().Property<string>("LastModificationUser");
            builder.Entity<T>().Property<string>("DeletionUser");

            builder.Entity<T>().HasOne<TUser>().WithMany().HasForeignKey("CreationUser").OnDelete(DeleteBehavior.Restrict);
            builder.Entity<T>().HasOne<TUser>().WithMany().HasForeignKey("LastModificationUser").OnDelete(DeleteBehavior.Restrict);
            builder.Entity<T>().HasOne<TUser>().WithMany().HasForeignKey("DeletionUser").OnDelete(DeleteBehavior.Restrict);
        }
    }
}
