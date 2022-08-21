using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace KYS.Library.Extensions
{
    public static class DataTableExtensions
    {
        public static bool IsNullOrEmpty(this DataTable dataTable)
            => dataTable.IsNull()
                || dataTable.IsEmpty();

        public static IList<T> ToList<T>(this DataTable dataTable)
        {
            if (dataTable.IsNull())
                return null;

            if (dataTable.IsEmpty())
                return default;

            IList<T> list = new List<T>();
            foreach (DataRow row in dataTable.Rows)
            {
                T item = GetItem<T>(row);
                list.Add(item);
            }

            return list;
        }

        #region Private Methods
        private static bool IsNull(this DataTable dataTable)
            => dataTable == null;

        private static bool IsEmpty(this DataTable dataTable)
            => dataTable.Rows.Count == 0;

        private static T GetItem<T>(DataRow row)
        {
            Type type = typeof(T);
            T obj = (T)Activator.CreateInstance(type);
            PropertyInfo[] propertyInfos = type.GetProperties();

            foreach (DataColumn column in row.Table.Columns)
            {
                PropertyInfo prop = propertyInfos.FirstOrDefault(x => x.ToDescription() == column.ColumnName);
                if (prop == null)
                    continue;

                Type propertyValueType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                var colValue = row[column.ColumnName];
                object safeValue = colValue != DBNull.Value
                    ? Convert.ChangeType(colValue, propertyValueType)
                    : null;
                prop.SetValue(obj, safeValue, null);
            }

            return obj;
        }
        #endregion
    }
}
