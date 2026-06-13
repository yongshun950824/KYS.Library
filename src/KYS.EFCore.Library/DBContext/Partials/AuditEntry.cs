using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using static KYS.Library.Helpers.FormattingHelper;

namespace KYS.EFCore.Library.DBContext.Partials
{
    public class AuditEntry
    {
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public AuditAction Action { get; set; }
        public Guid? UserId { get; set; }
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
        public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();
        public bool HasTemporaryProperties => TemporaryProperties.Count > 0;

        private static readonly JsonMergeSettings _mergeSettings = new JsonMergeSettings
        {
            MergeArrayHandling = MergeArrayHandling.Union
        };

        private static readonly JsonSerializerSettings _defaultJsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new DefaultNamingStrategy()
            }
        };

        private static readonly JsonSerializerSettings _snakeCaseJsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy { ProcessDictionaryKeys = true }
            }
        };

        public Result<ActionLog> ToActionLog(KYS.Library.Helpers.FormattingHelper.Formatting? formatting = null)
        {
            string formattedTableName = TableName;
            string formattedColumnName = ColumnName;

            if (formatting != null)
            {
                var tableNameResult = TableName.Convert(formatting.Value);
                if (tableNameResult.IsFailure)
                    return Result.Failure<ActionLog>($"Failed to convert table name '{TableName}' with error: {tableNameResult.Error}");

                var columnNameResult = ColumnName.Convert(formatting.Value);
                if (columnNameResult.IsFailure)
                    return Result.Failure<ActionLog>($"Failed to convert column name '{ColumnName}' with error: {columnNameResult.Error}");

                formattedTableName = tableNameResult.Value;
                formattedColumnName = columnNameResult.Value;
            }

            var auditLog = new ActionLog
            {
                ReferenceId = KeyValues.FirstOrDefault().Value.ToString(),
                ReferenceTable = formattedTableName,
                ColumnName = formattedColumnName,
                ActionType = Action,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = UserId
            };

            JObject recordValueJObj;
            JsonSerializerSettings jsonSerializerSettings = formatting switch
            {
                KYS.Library.Helpers.FormattingHelper.Formatting.SnakeCase => _snakeCaseJsonSerializerSettings,
                _ => _defaultJsonSerializerSettings
            };

            switch (Action)
            {
                case AuditAction.Insert:
                    recordValueJObj = JObject.FromObject(KeyValues);
                    recordValueJObj.Merge(JObject.FromObject(NewValues), _mergeSettings);

                    auditLog.RecordValue = JsonConvert.SerializeObject(recordValueJObj.ToObject<ExpandoObject>(),
                        jsonSerializerSettings);

                    break;

                case AuditAction.Update:
                    auditLog.ColumnOldValue = OldValues.First(x => x.Key == ColumnName)
                        .Value
                        .ToString();
                    auditLog.ColumnNewValue = NewValues.First(x => x.Key == ColumnName)
                        .Value
                        .ToString();

                    break;

                case AuditAction.Delete:
                    recordValueJObj = JObject.FromObject(KeyValues);
                    recordValueJObj.Merge(JObject.FromObject(OldValues), _mergeSettings);

                    auditLog.RecordValue = JsonConvert.SerializeObject(recordValueJObj.ToObject<ExpandoObject>(),
                        jsonSerializerSettings);

                    break;
            }

            return auditLog;
        }
    }
}
