using Microsoft.AspNetCore.Mvc;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Diagnostics;
using SQLOperation.PublicAccess.Templates.SQLManager;
using Newtonsoft.Json.Linq;
using SQLOperation.PublicAccess.Utilities;

namespace WebAppTest.APITemplate
{
    //[Route("api/[controller]")]
    //这是restful风格

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
        //public SQLOpsController(string Uid = "cjh2251646", string Password = "123456", string DataSource = "localhost:1521/orclpdb")
        //{
        //    conn = new Connection(Uid, Password, DataSource);
        //    OracleConnection = conn.GetOracleConnection();
        //    SQLOps = new BasicSQLOps(conn);
        //}

        public SQLOpsController(string Uid = "ADMIN", string Password = "123456", string DataSource = "121.36.200.128:1521/ORCL")
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
                //result = SQLOps.QueryOperation("instructor", "salary", 40000);
                result = SQLOps.QueryOperation("auth_info", "user_id", 400);
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
        public bool Insert([FromBody] dynamic auth_info)
        {

            if (OracleConnection.State != ConnectionState.Open)
                OracleConnection.Open();
            try
            {
                // 将前端传入的json数据解析为数据类的流程
                string JsonString = auth_info.ToString();
                Debug.WriteLine("received json is " + JsonString);
                JObject JO = JObject.Parse(JsonString);

                Auth_Info AuthInfo = new Auth_Info();
                try
                {
                    AuthInfo = JO.ToObject<Auth_Info>();
                }
                catch(Exception ex)
                {
                    Debug.WriteLine($"Json Deserialization Error: {ex.Message}");
                    return false;
                }
                // 剩下的部分应该出现在业务逻辑层中，这里只是为了方便测试所以写在APi层了
                List<string> ColumnNames = new List<string>() {"User_id","auth_status","auth_date","status"};
                List<Object> Values = new List<Object>() {AuthInfo.User_ID,AuthInfo.Auth_Status,AuthInfo.Auth_Date,AuthInfo.Status };
                //bool InsertStatus = SQLOps.InsertOperation("auth_info", new List<string> { "user_id", "auth_status", "auth_date", "status" }, new List<object> { 400, "not bad", new DateTime(2024, 7, 10, 12, 58, 30), "not approved" });
                bool InsertStatus = SQLOps.InsertOperation("auth_info", ColumnNames, Values);
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
            if (OracleConnection.State != ConnectionState.Open)
                OracleConnection.Open();
            try
            {
                bool DeleteStatus = false;
                //DeleteStatus = SQLOps.DeleteOperation("TestTable", "ID", "2150988");
                DeleteStatus = SQLOps.DeleteOperation("auth_info", "user_id", 400);
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
                //bool UpdateStatus = SQLOps.UpdateOperation("TestTable", "Name", "hkj", "ID", "2150987");
                bool UpdateStatus = SQLOps.UpdateOperation("auth_info", "auth_status", "ok", "user_id", 400);
                return UpdateStatus;
            }
            catch (Exception ex)
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
