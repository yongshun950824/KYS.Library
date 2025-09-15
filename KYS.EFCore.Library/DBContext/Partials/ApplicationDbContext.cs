using KYS.EFCore.Library.DBContext.Partials;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static KYS.Library.Helpers.FormattingHelper;

namespace KYS.EFCore.Library.DBContext
{
    public partial class ApplicationDbContext
    {
        public async Task<int> SaveChangesWithAuditAsync(Guid? actionBy = null
            , FormattingEnum formatting = FormattingEnum.SnakeCase)
        {
            ChangeTracker.DetectChanges();
            var auditEntries = OnBeforeSaveChanges(actionBy, formatting);
            var result = await base.SaveChangesAsync();
            await OnAfterSaveChangesAsync(auditEntries, formatting);
            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges(Guid? actionBy, FormattingEnum formatting)
        {
            try
            {
                var auditEntries = new List<AuditEntry>();
                foreach (var entry in ChangeTracker.Entries())
                {
                    if (!ShouldAuditEntry(entry))
                        continue;

                    var auditEntry = new AuditEntry(entry)
                    {
                        TableName = entry.Entity.GetType().Name,
                        UserId = actionBy
                    };
                    auditEntries.Add(auditEntry);

                    foreach (var property in entry.Properties)
                    {
                        ProcessProperty(entry, property, auditEntry);
                    }
                }

                var noTempPropAuditEntries = auditEntries.Where(e => !e.HasTemporaryProperties).ToList();
                foreach (var auditEntry in noTempPropAuditEntries)
                {
                    ActionLog.Add(auditEntry.ToActionLog(formatting));
                }

                return auditEntries.Where(e => e.HasTemporaryProperties).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{Method}] Exception", nameof(OnBeforeSaveChanges));

                return new List<AuditEntry>();
            }
        }

        private async Task OnAfterSaveChangesAsync(List<AuditEntry> auditEntries, FormattingEnum formatting)
        {
            try
            {
                if (auditEntries == null || auditEntries.Count == 0)
                    return;

                foreach (var auditEntry in auditEntries)
                {
                    foreach (var prop in auditEntry.TemporaryProperties)
                    {
                        if (prop.Metadata.IsPrimaryKey())
                        {
                            auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                        }
                        else
                        {
                            auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                        }
                    }

                    ActionLog.Add(auditEntry.ToActionLog(formatting));
                }

                await base.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{Method}] Exception", nameof(OnAfterSaveChangesAsync));
            }
        }

        private static bool ShouldAuditEntry(EntityEntry entry)
            => !(entry.Entity is ActionLog
                || entry.State == EntityState.Detached
                || entry.State == EntityState.Unchanged);

        private static void ProcessProperty(EntityEntry entry, PropertyEntry property, AuditEntry auditEntry)
        {
            if (property.IsTemporary)
            {
                auditEntry.TemporaryProperties.Add(property);
                return;
            }

            var propertyName = property.Metadata.Name;
            if (property.Metadata.IsPrimaryKey())
            {
                auditEntry.KeyValues[propertyName] = property.CurrentValue;
                return;
            }

            ApplyStateChange(entry, property, auditEntry, propertyName);
        }

        private static void ApplyStateChange(EntityEntry entry, PropertyEntry property, AuditEntry auditEntry, string propertyName)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    auditEntry.Action = "INSERT";
                    auditEntry.NewValues[propertyName] = property.CurrentValue;
                    break;

                case EntityState.Deleted:
                    auditEntry.Action = "DELETE";
                    auditEntry.OldValues[propertyName] = property.OriginalValue;
                    break;

                case EntityState.Modified:
                    auditEntry.Action = "UPDATE";
                    if (property.IsModified && !Equals(property.OriginalValue, property.CurrentValue))
                    {
                        auditEntry.Column = propertyName;
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                    }
                    break;
            }
        }
    }
}
