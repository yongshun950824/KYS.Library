using KYS.EFCore.Library.DBContext.Partials;
using KYS.Library.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static KYS.Library.Helpers.FormattingHelper;

namespace KYS.EFCore.Library.DBContext
{
    public partial class ApplicationDbContext
    {
        public async Task<int> SaveChangesWithAuditAsync(Guid? actionBy = null,
            Formatting? formatting = null)
        {
            ChangeTracker.DetectChanges();
            var auditEntries = OnBeforeSaveChanges(actionBy, formatting);
            var result = await base.SaveChangesAsync();
            await OnAfterSaveChangesAsync(auditEntries, formatting);
            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges(Guid? actionBy, Formatting? formatting)
        {
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (!ShouldAuditEntry(entry))
                    continue;

                var auditEntry = new AuditEntry
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
                var result = auditEntry.ToActionLog(formatting);
                if (result.IsFailure)
                    throw new InvalidOperationException(result.Error);

                ActionLog.Add(result.Value);
            }

            return auditEntries.Where(e => e.HasTemporaryProperties).ToList();
        }

        private async Task OnAfterSaveChangesAsync(List<AuditEntry> auditEntries, Formatting? formatting)
        {
            if (auditEntries.IsNullOrEmpty())
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

                var result = auditEntry.ToActionLog(formatting);
                if (result.IsFailure)
                    throw new InvalidOperationException(result.Error);

                ActionLog.Add(result.Value);
            }

            await base.SaveChangesAsync();
        }

        private static bool ShouldAuditEntry(EntityEntry entry)
            => !(entry.Entity is ActionLog
                || entry.State == EntityState.Detached
                || entry.State == EntityState.Unchanged);

        private static void ProcessProperty(EntityEntry entry, PropertyEntry property, AuditEntry auditEntry)
        {
            // Treat database-generated properties as temporary. Some providers
            // (SQLite) may not mark non-PK store-generated columns as
            // PropertyEntry.IsTemporary; additionally treat properties with
            // ValueGenerated.OnAdd and no client value as temporary so the
            // post-save step can capture the generated value.
            var isStoreGeneratedOnAdd = property.Metadata.ValueGenerated == Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.OnAdd
                                         && property.CurrentValue == null;

            if (property.IsTemporary || isStoreGeneratedOnAdd)
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
                    auditEntry.Action = AuditAction.Insert;
                    auditEntry.NewValues[propertyName] = property.CurrentValue;
                    break;

                case EntityState.Deleted:
                    auditEntry.Action = AuditAction.Delete;
                    auditEntry.OldValues[propertyName] = property.OriginalValue;
                    break;

                case EntityState.Modified:
                    auditEntry.Action = AuditAction.Update;
                    if (property.IsModified && !Equals(property.OriginalValue, property.CurrentValue))
                    {
                        auditEntry.ColumnName = propertyName;
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                    }
                    break;
            }
        }
    }
}
