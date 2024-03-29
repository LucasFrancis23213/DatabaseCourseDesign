using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Templates
{
    public class Connection
    {
        private string Uid;//user id,即用户名
        private string Password;//密码
        private string DataSource;//连接哪个数据库
        //数据库是否成功连接，默认为未连接
        public bool IsConnected { get; private set; } = false;
        //检测数据库是否连接成功的实例化对象
        private OracleConnection OracleConnection;

        //构造函数
        public Connection(string Uid,string Password,string DataSource)
        {
            this.Uid = Uid;
            this.Password = Password;
            this.DataSource = DataSource;
            IsConnected = ConnectSQL();
        }
        //连接到指定数据库
        private bool ConnectSQL()
        {
            string ConnectString = $"User Id={Uid};Password={Password};Data Source={DataSource};";
            this.OracleConnection = new OracleConnection(ConnectString);
            try
            {
                OracleConnection.Open();
                Debug.WriteLine("数据库连接成功");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"数据库连接失败，报错为{ex}");
                return false;
            }
        } 

        public void DisconnectSQL()
        {
            if (IsConnected)
            {
                OracleConnection.Close();
                Debug.WriteLine("数据库已成功关闭");
            }
        }


    }


}
