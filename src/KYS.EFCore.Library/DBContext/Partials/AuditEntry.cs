using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static KYS.Library.Helpers.FormattingHelper;

namespace KYS.EFCore.Library.DBContext.Partials
{
    public class AuditEntry
    {
        public AuditEntry(EntityEntry entry)
        {
            Entry = entry;
        }

        public EntityEntry Entry { get; }
        public string TableName { get; set; }
        public string Column { get; set; }
        public string Action { get; set; }
        public Guid? UserId { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();
        public bool HasTemporaryProperties => TemporaryProperties.Count > 0;

        public Result<ActionLog> ToActionLog(KYS.Library.Helpers.FormattingHelper.Formatting formatting = KYS.Library.Helpers.FormattingHelper.Formatting.SnakeCase)
        {
            var tableNameResult = TableName.Convert(formatting);
            if (tableNameResult.IsFailure)
                return Result.Failure<ActionLog>($"Failed to convert table name '{TableName}' with error: {tableNameResult.Error}");

            var columnNameResult = Column.Convert(formatting);
            if (columnNameResult.IsFailure)
                return Result.Failure<ActionLog>($"Failed to convert column name '{Column}' with error: {columnNameResult.Error}");

            var auditLog = new ActionLog
            {
                ReferenceId = KeyValues.FirstOrDefault().Value.ToString(),
                ReferenceTable = tableNameResult.Value,
                ColumnName = columnNameResult.Value,
                ActionType = Action,
                CreatedDate = DateTime.UtcNow,
                CreatedUser = UserId
            };

            var mergeSettings = new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            };

            JObject recordValueJObj;

            switch (Action.ToUpper())
            {
                case "INSERT":
                    recordValueJObj = JObject.FromObject(KeyValues);
                    recordValueJObj.Merge(JObject.FromObject(NewValues), mergeSettings);

                    auditLog.RecordValue = JsonConvert.SerializeObject(recordValueJObj);
                    auditLog.ColumnOldValue = null;
                    auditLog.ColumnNewValue = null;

                    break;

                case "UPDATE":
                    auditLog.ColumnOldValue = OldValues.First(x => x.Key == Column)
                        .Value
                        .ToString();
                    auditLog.ColumnNewValue = NewValues.First(x => x.Key == Column)
                        .Value
                        .ToString();

                    break;

                case "DELETE":
                    recordValueJObj = JObject.FromObject(KeyValues);
                    recordValueJObj.Merge(JObject.FromObject(OldValues), mergeSettings);

                    auditLog.RecordValue = JsonConvert.SerializeObject(recordValueJObj);
                    auditLog.ColumnOldValue = null;
                    auditLog.ColumnNewValue = null;

                    break;
            }

            return auditLog;
        }
    }
}
