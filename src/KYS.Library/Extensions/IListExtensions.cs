using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace KYS.Library.Extensions
{
    /// <summary>
    /// Provides extension methods for working with <see cref="IList{T}"/>.
    /// </summary>
    public static class IListExtensions
    {
        /// <summary>
        /// Convert <see cref="IList{T}"/> instance to <see cref="DataTable" /> instance.
        /// </summary>
        /// <typeparam name="T">The type of element in the <c>list</c>.</typeparam>
        /// <param name="list">The <see cref="IList{T}"/> instance this method extends.</param>
        /// <returns>The <see cref="DataTable" /> instance consists of the element(s) in the <c>list</c>.</returns>
        public static DataTable ToDataTable<T>(this IList<T> list)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable dataTable = new DataTable();

            foreach (PropertyDescriptor property in properties)
                dataTable.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);

            foreach (T item in list)
            {
                DataRow row = dataTable.NewRow();
                foreach (PropertyDescriptor property in properties)
                    row[property.Name] = property.GetValue(item) ?? DBNull.Value;

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
