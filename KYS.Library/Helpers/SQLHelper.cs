using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace KYS.Library.Helpers
{
    /// <summary>
    /// Provide utility methods for ADO.NET.
    /// </summary>
    public static class SqlHelper
    {
        /// <summary>
        /// Execute SQL query and obtain the result in <see cref="DataSet" />.
        /// </summary>
        /// <param name="connectionString">The connection string for database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="commandText">The command query.</param>
        /// <param name="sqlParameters">One or more <see cref="SqlParameter" />s.</param>
        /// <returns>The <see cref="DataSet" /> instance containing the result for the query.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static DataSet GetDataSet(string connectionString, CommandType commandType, string commandText,
            params SqlParameter[] sqlParameters)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
            ArgumentException.ThrowIfNullOrWhiteSpace(commandText);

            using SqlConnection con = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(commandText, con);
            con.Open();
            command.CommandType = commandType;
            command.Parameters.AddRange(sqlParameters);

            using SqlDataAdapter da = new SqlDataAdapter(command);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return ds;
        }

        /// <summary>
        /// Execute SQL query and obtain the result in <see cref="DataTable" />.
        /// </summary>
        /// <param name="connectionString">The connection string for database.</param>
        /// <param name="commandType">The command type.</param>
        /// <param name="commandText">The command query.</param>
        /// <param name="sqlParameters">One or more <see cref="SqlParameter" />s.</param>
        /// <returns>The <see cref="DataTable" /> instance containing the result for the query.</returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static DataTable GetDataTable(string connectionString, CommandType commandType, string commandText,
            params SqlParameter[] sqlParameters)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);
            ArgumentException.ThrowIfNullOrWhiteSpace(commandText);

            using SqlConnection con = new SqlConnection(connectionString);
            using SqlCommand command = new SqlCommand(commandText, con);
            con.Open();
            command.CommandType = commandType;
            command.Parameters.AddRange(sqlParameters);

            using SqlDataAdapter da = new SqlDataAdapter(command);
            DataTable dt = new DataTable();
            da.Fill(dt);

            return dt;
        }

        /// <summary>
        /// Build structured parameter. Scenario: Query with <c>IN (SELECT [col] FROM @Parameter)</c>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tableTypeName">Name of Table Type created in database.</param>
        /// <param name="tableTypeColumnName">Column name in Table Type.</param>
        /// <param name="parameterName">Parameter name.</param>
        /// <param name="values">One or more values as the parameter value(s).</param>
        /// <returns>The <see cref="SqlParameter"/> instance for the table-valued parameter.</returns>
        /// <remarks>
        /// <para><b>Pre-requisites</b></para>
        /// <list type="number">
        ///   <item>
        ///     <description>
        ///       Create a table type in the database.<br />
        ///       SQL: <c>CREATE TYPE dbo.IdList AS TABLE ([Id] int PRIMARY KEY)</c>
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       Use the table type in your SQL query:<br />
        ///       <c>
        ///       SELECT [col] 
        ///       FROM [table] 
        ///       WHERE [col] IN (SELECT [tableTypeColumnName] FROM @Parameter)
        ///       </c>
        ///     </description>
        ///   </item>
        /// </list>
        ///
        /// <para><b>References</b></para>
        /// <list type="number">
        ///   <item>
        ///     <description>
        ///       <a href="https://dotnetfiddle.net/jOePEn">Demo</a>
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <a href="https://stackoverflow.com/questions/73561484/ado-net-query-returning-nothing-even-if-the-item-available">
        ///         StackOverflow: ADO.NET - Query with <c>IN()</c>
        ///       </a>
        ///     </description>
        ///   </item>
        /// </list>
        /// </remarks>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static SqlParameter BuildStructuredSqlParameter<T>(string tableTypeName, string tableTypeColumnName,
            string parameterName, IEnumerable<T> values)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(tableTypeName);
            ArgumentException.ThrowIfNullOrWhiteSpace(tableTypeColumnName);
            ArgumentException.ThrowIfNullOrWhiteSpace(parameterName);

            DataTable dt = new DataTable();
            dt.Columns.Add(tableTypeColumnName, typeof(T));

            foreach (var @value in values)
                dt.Rows.Add(new object[] { @value });

            return new SqlParameter(parameterName, SqlDbType.Structured)
            {
                Value = dt,
                TypeName = tableTypeName,
            };
        }
    }
}
