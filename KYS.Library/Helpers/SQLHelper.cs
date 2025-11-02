using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace KYS.Library.Helpers
{
    public static class SqlHelper
    {
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
        /// <param name="values">One or more values as the paramter value(s).</param>
        /// <returns></returns>
        /// <remarks>
        /// <b>Pre-requisites</b> <br />
        /// 1. Create a Table Type in database. <br />
        /// SQL: <c>CREATE TYPE dbo.IdList AS TABLE ([Id] int PRIMARY KEY)</c> <br />
        /// 2. SqlCommand: <br />
        /// <c>SELECT [col] FROM [table] WHERE [col] IN (SELECT [tableTypeColumnName] FROM @Parameter)</c>
        /// <br /><br />
        /// <b>References</b>  <br />
        /// 1. <a href="https://dotnetfiddle.net/jOePEn">Demo</a> <br />
        /// 2. <a href="https://stackoverflow.com/questions/73561484/ado-net-query-returning-nothing-even-if-the-item-available">
        ///     StackOverflow: ADO.NET - Query with <c>IN ()</c>
        ///    </a>
        /// </remarks>
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
