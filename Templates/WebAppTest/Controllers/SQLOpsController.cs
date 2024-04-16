using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Diagnostics;
using SQLOperation.SQLManager;

namespace WebAppTest.Controllers
{
    //[Route("api/[controller]")]//这是restful风格

    //这种就是直接在路由上就展示业务实现（展示函数名）
    [Route("api/[controller]/[action]")]

    // get方法代表向数据库查询(select)
    // post方法代表向数据库插入(insert into)
    // put方法代表向数据库更新数据(update)
    // delete方法用于在数据库里删除数据(delete)

    [ApiController]
    public class SQLOpsController : ControllerBase
    {
        private Connection conn;
        private OracleConnection OracleConnection;
        private BasicSQLOps SQLOps;
        public SQLOpsController(string Uid = "cjh2251646", string Password = "123456", string DataSource = "localhost:1521/orclpdb")
        {
            conn = new Connection(Uid, Password, DataSource);
            OracleConnection = conn.GetOracleConnection();
            SQLOps = new BasicSQLOps(conn);
        }

        [HttpGet]
        public string Query()
        {
            if (OracleConnection.State != ConnectionState.Open)
                OracleConnection.Open();
            try
            {
                string result = string.Empty;
                result = SQLOps.QueryOperation("instructor", "salary", 40000);
                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"In Query Function,报错为：{ex}");
                return ex.Message;
            }
            finally
            {
                OracleConnection.Close();
            }
            
        }

        [HttpPost]
        public bool Insert() 
        {
            
            if (OracleConnection.State != ConnectionState.Open)
                OracleConnection.Open();
            try
            {
                bool InsertStatus = SQLOps.InsertOperation("TestTable", "ID", "2150988");
                return InsertStatus;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                OracleConnection.Close();
            }
            
        }

        [HttpDelete]
        public bool Delete()
        {
            if(OracleConnection.State != ConnectionState.Open)
                OracleConnection.Open();
            try
            {
                bool DeleteStatus = false;
                DeleteStatus = SQLOps.DeleteOperation("TestTable", "ID", "2150988");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"In Delete Function报错，报错信息为{ex}");
                return false;
            }
            finally
            {
                OracleConnection.Close();
            }
        }

        [HttpPut]
        public bool Update()
        {
            if (OracleConnection.State != ConnectionState.Open)
                OracleConnection.Open();
            try
            {
                bool UpdateStatus = SQLOps.UpdateOperation("TestTable", "Name", "hkj", "ID", "2150987");
                return UpdateStatus;
            }
            catch(Exception ex)
            {
                Debug.WriteLine($"In Update Function,报错:{ex.Message}");
                return false;
            }
            finally
            {
                OracleConnection.Close();
            }
            
        }
    }
}
