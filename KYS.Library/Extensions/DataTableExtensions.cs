using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace KYS.Library.Extensions
{
    /// <summary>
    /// Provides extension methods for working with <see cref="DataTable" />.
    /// </summary>
    public static class DataTableExtensions
    {
        /// <summary>
        /// Check <see cref="DataTable" /> instance is null or empty.
        /// </summary>
        /// <param name="dataTable">The <see cref="DataTable" /> instance this method extends.</param>
        /// <returns>The <see cref="bool" /> value indicates the <c>dataTable</c> is null or empty.</returns>
        public static bool IsNullOrEmpty(this DataTable dataTable)
            => dataTable.IsNull()
                || dataTable.IsEmpty();

        /// <summary>
        /// Retrieve the element(s) from the <see cref="DataTable" /> instance and return the element(s) as <see cref="IList{T}"/> type.
        /// </summary>
        /// <typeparam name="T">The type of element for the returned element(s).</typeparam>
        /// <param name="dataTable">The <see cref="DataTable" /> instance this method extends.</param>
        /// <returns>The <see cref="IList{T}" /> instance containing element(s) retrieved from the <c>dataTable</c>.</returns>
        public static IList<T> ToList<T>(this DataTable dataTable)
            where T : class, new()
        {
            if (dataTable.IsNull() || dataTable.IsEmpty())
                return new List<T>();

            List<T> list = new List<T>();
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
                PropertyInfo prop = propertyInfos.FirstOrDefault(x => x.ToName() == column.ColumnName);
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
