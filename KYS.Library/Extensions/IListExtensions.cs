using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace KYS.Library.Extensions
{
    public static class IListExtensions
    {
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
