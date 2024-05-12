using Oracle.ManagedDataAccess.Client;
using SQLOperation.InterfaceManager;
using SqlSugar.Extensions;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;


namespace SQLOperation.SQLManager
{
    public class Assistance : IAssistance
    {
        private OracleConnection OracleConnection;
        public Assistance(OracleConnection conn)
        {
            OracleConnection = conn;
        }
        public bool CheckColumnExists(string ColumnName, string TableName)
        {
            if (OracleConnection.State == ConnectionState.Open)
            {
                ColumnName = ColumnName.ToUpper();
                TableName = TableName.ToUpper();
                string Query = "SELECT COUNT(*) FROM ALL_TAB_COLUMNS WHERE TABLE_NAME = :TableName AND COLUMN_NAME = :ColumnName";
                using (OracleCommand cmd = new OracleCommand(Query, OracleConnection))
                {
                    cmd.Parameters.Add(new OracleParameter(":TableName", TableName));
                    cmd.Parameters.Add(new OracleParameter(":ColumnName", ColumnName));
                    try
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            int count = Convert.ToInt32(result);
                            return count > 0;
                        }
                        return false;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        Debug.WriteLine(ex.ToString());
                        return false;
                    }
                }
            }
            else
            {
                Debug.WriteLine("In CheckColumnExists Function，数据库未连接");
                return false;
            }
        }
    }
}
