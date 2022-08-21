using System.Data;
using System.Data.SqlClient;

namespace KYS.Library.Helpers
{
    public static class SQLHelper
    {
        public static DataSet GetDataSet(string connectionString, CommandType commandType, string commandText,
            params SqlParameter[] sqlParameters)
        {
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
    }
}
