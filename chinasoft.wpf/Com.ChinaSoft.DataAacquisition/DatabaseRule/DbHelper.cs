using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Com.ChinaSoft.DataAcquisition.DatabaseRule
{
    /// <summary>
    /// INSQL数据库访问类
    /// </summary>
    public class DbHelper
    {
        private static string ConnectionString =
            ConfigurationManager.ConnectionStrings["ConnectionStringNameINSQL"].ConnectionString;

        public static DataSet Query(string sql, params SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = connection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.CommandType = CommandType.Text;
                    if (parameters?.Length > 0) cmd.Parameters.Add(parameters);

                    IDbDataAdapter adapter = new SqlDataAdapter(cmd);

                    DataSet ds = new DataSet();

                    adapter.Fill(ds);
                    return ds;
                }
            }
        }

        public static object DbNullToObject(object obj)
        {
            if (obj == DBNull.Value)
                return null;

            return obj;
        }
    }
}