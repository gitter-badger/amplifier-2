using Amplifier.AspNetCore.Auditing;
using Amplifier.AspNetCore.Authentication;
using Amplifier.AspNetCore.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace Amplifier.EntityFrameworkCore
{
    /// <summary>
    /// ChangeTracker extension methods.
    /// </summary>
    public static class ChangeTrackerExtensions
    {
        /// <summary>
        /// Enable assignment of tenantid and audit properties automatically.
        /// <param name="changeTracker"></param>
        /// <param name="userSession"></param>
        /// <typeparam name="TTenantKey">Tenant primary key type</typeparam>
        /// <typeparam name="TUserKey">USer primary key type</typeparam>
        /// </summary>
        public static void AutomaticTenantIdAndAuditing<TTenantKey, TUserKey>(this ChangeTracker changeTracker, IUserSession<TTenantKey, TUserKey> userSession)
        {
            changeTracker.DetectChanges();

            var timestamp = DateTime.Now;

            foreach (var entry in changeTracker.Entries())
            {
                if (entry.Entity is IFullAuditedEntity)
                {
                    if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                    {
                        entry.Property("LastModificationTime").CurrentValue = timestamp;
                        entry.Property("LastModificationUser").CurrentValue = userSession.UserId;
                    }

                    if (entry.State == EntityState.Added)
                    {
                        entry.Property("CreationTime").CurrentValue = timestamp;
                        entry.Property("CreationUser").CurrentValue = userSession.UserId;
                    }
                }

                if (entry.Entity is IHaveTenant || entry.Entity is IMayHaveTenant)
                {
                    entry.Property("TenantId").CurrentValue = userSession.TenantId;
                }

                if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete)
                {
                    entry.State = EntityState.Modified;
                    entry.Property("IsDeleted").CurrentValue = true;
                    entry.Property("DeletionTime").CurrentValue = timestamp;
                    entry.Property("DeletionUser").CurrentValue = userSession.UserId;
                }
            }
        }
    }
}
