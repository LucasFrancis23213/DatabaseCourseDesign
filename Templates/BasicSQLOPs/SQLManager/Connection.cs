using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Diagnostics;
//using System.Data.OracleClient;

namespace SQLOperation.SQLManager
{
    public class Connection
    {
        private string Uid;//user id,即用户名
        private string Password;//密码
        private string DataSource;//连接哪个数据库

        //检测数据库是否连接成功的实例化对象
        private OracleConnection OracleConnection;

        //构造函数
        public Connection(string Uid, string Password, string DataSource)
        {
            this.Uid = Uid;
            this.Password = Password;
            this.DataSource = DataSource;
            ConnectSQL();
        }
        //连接到指定数据库
        private bool ConnectSQL()
        {
            string ConnectString = $"Data Source={DataSource}; User Id={Uid}; Password={Password};";
            //if(OracleConnection.State != ConnectionState.Open)
            //OracleConfiguration.WalletLocation = "D:\\OracleBase\\admin\\OralceMajorDatabase";
            Console.WriteLine(ConnectString);
            OracleConnection = new OracleConnection(ConnectString);
            try
            {
                OracleConnection.Open();
                Debug.WriteLine("数据库连接成功");
                Console.WriteLine("数据库连接成功");
                return true;
            }
            catch (OracleException ex)
            {
                //Debug.WriteLine($"数据库连接失败，报错为{ex}");
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine("Error Code: " + ex.ErrorCode);
                Console.WriteLine("Details: " + ex.StackTrace);
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        public OracleConnection GetOracleConnection()
        {
            return OracleConnection;
        }
        public void DisconnectSQL()
        {
            if (OracleConnection.State == ConnectionState.Open)
            {
                OracleConnection.Close();
                Debug.WriteLine("数据库已成功关闭");
            }
        }


    }


}
