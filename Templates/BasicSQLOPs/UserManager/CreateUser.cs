using System;
using System.Data;
using System.Diagnostics;
using Oracle.ManagedDataAccess.Client;
using SQLOperation.SQLManager;
using SQLOperation.Utilities;

namespace SQLOperation.UserManager
{
    public class CreateUser
    {
        private string ManagerName { get; set; } = "SYSTEM";
        private string ManagerPassword { get; set; } = "Ss504754572";
        private string ManagerDataSource { get; set; } = "localhost:1521/orclpdb";
        private string UserName { get; set; }
        private string UserPassword { get; set; }
        private string TablesGranted { get; set; } = "cjh2251646.INSTRUCTOR";
        private string UserDataSource { get; set; } = "localhost:1521/orclpdb";//pdb数据库名
        private OracleConnection ManagerConnection { get; set; }

        private bool _CreateStatus { get; set; } = false;
        //创建账号的状态，成功创建为true,反之为false
        //为了实现类内可读可写，类外只读；使用这种手写getter和setter的方法
        public bool CreateStatus
        {
            get { return _CreateStatus; }
            private set { _CreateStatus = value; }
        }
        private string _ReasonForCreationFailure { get; set; } = string.Empty;
        //返回账号创建失败的原因
        public string ReasonForCreationFailure
        {
            get { return _ReasonForCreationFailure; }
            private set { _ReasonForCreationFailure = value; }
        }
        private Connection Connection;
        public CreateUser(UserInfo UserInfo)
        {
            UserName = UserInfo.UserName;
            UserPassword = UserInfo.Password;
            Connection = new Connection(ManagerName, ManagerPassword, ManagerDataSource);
            ManagerConnection = Connection.GetOracleConnection();
            if (!UserExists())
                _CreateStatus = UserCreation();
        }
        private bool UserCreation()
        {
            if (ManagerConnection.State != ConnectionState.Open)
                ManagerConnection.Open();
            try
            {
                using (OracleCommand cmd = ManagerConnection.CreateCommand())
                {
                    //创建新用户
                    cmd.CommandText = $"CREATE USER {UserName} IDENTIFIED BY {UserPassword}";
                    cmd.ExecuteNonQuery();
                    //授予用户连接权限
                    cmd.CommandText = $"GRANT CREATE SESSION TO {UserName}";
                    cmd.ExecuteNonQuery();
                    //授予用户在某些表的读写权限
                    cmd.CommandText = $"GRANT SELECT,INSERT,UPDATE,DELETE ON {TablesGranted} TO {UserName}";
                }
                Debug.WriteLine($"{UserName}已成功创建并被分配权限");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("In Create User,报错：" + ex);
                _ReasonForCreationFailure += ex.Message;
                return false;
            }
            finally
            {
                ManagerConnection.Close();
            }
        }
        //检查当前用户是否被创建
        //返回值:true代表已被创建，false代表未被创建
        public bool UserExists()
        {
            bool Exists = false;
            try
            {
                if (ManagerConnection.State != ConnectionState.Open)
                    ManagerConnection.Open();
                string CommandStr = $"SELECT COUNT(*) FROM ALL_USERS WHERE USERNAME = '{UserName.ToUpper()}'";
                using (OracleCommand cmd = new OracleCommand(CommandStr, ManagerConnection))
                {
                    int UserCount = Convert.ToInt32(cmd.ExecuteScalar());
                    if (UserCount > 0)
                    {
                        Exists = true;
                        _ReasonForCreationFailure += $"用户{UserName}已经存在";
                    }
                    else
                    {
                        Exists = false;
                    }

                }
                return Exists;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("In UserExist Function,报错" + ex);
                _ReasonForCreationFailure += ex.Message;
                return false;
            }
        }

    }
}
