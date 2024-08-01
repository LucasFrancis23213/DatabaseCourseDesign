using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Templates.TemplateInterfaceManager;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Drawing;
using Newtonsoft.Json;


namespace SQLOperation.BusinessLogicLayer.BasicFeatureBLL
{

    public class PublishItem : IPublishItem
    {
        private Connection conn;
        private OracleConnection OracleConnection;
        //private BasicSQLOps SQLOps;

        public PublishItem(string Uid = "ADMIN", string Password = "123456", string DataSource = "121.36.200.128:1521/ORCL")
        {
            conn = new Connection(Uid, Password, DataSource);
            OracleConnection = conn.GetOracleConnection();
            //SQLOps = new BasicSQLOps(conn);
        }
        //发布寻物启事基础表单
        private Tuple<bool, string> PublishLostItemBasic(Lost_Item item)
        {
            var lostNames = new List<string>
            {
            "Item_ID",
            "Item_Name",
            "Category_ID",
            "Description",
            "Lost_Location",
            "Lost_Date",
            "User_ID",
            "Lost_Status",
            "Review_Status",
            "Image_URL",
            "Tag_ID"
            };
            var values = new List<object>
            {
                item.Item_ID,
                item.Item_Name,
                item.Category_ID,
                item.Description,
                item.Lost_Location,
                item.Lost_Date,
                item.User_ID,
                item.Lost_Status,
                item.Review_Status,
                item.Image_URL,
                item.Tag_ID
            };
            BasicSQLOps basic = new BasicSQLOps(conn);
            var result = basic.InsertOperation("Lost_Items", lostNames, values);
            bool isSuccess = result.Item1; // 获取是否成功插入
            string errorReason = result.Item2; // 获取出错误原因
            if (isSuccess)
                return new Tuple<bool, string>(true, string.Empty);
            //插入失败
            else
            {
                return new Tuple<bool, string>(false, errorReason);
            }
        }
        //选择设置悬赏找物品
        private Tuple<bool, string> HaveReward(Reward_Offers item)
        {
            var Names = new List<string>
            {
            "User_ID",
            "Item_ID",
            "Reward_Amount",
            "Status",
            "Release_Date",
            "Deadline"
            };
            var values = new List<object>
            {
                item.User_ID,
                item.Item_ID,
                item.Reward_Amount,
                item.Status,
                item.Release_Date,
                item.Deadline,
            };
            BasicSQLOps basic = new BasicSQLOps(conn);
            var result = basic.InsertOperation("Reward_Offers", Names, values);
            bool isSuccess = result.Item1; // 获取是否成功插入
            string errorReason = result.Item2; // 获取出错误原因
            if (isSuccess)
                return new Tuple<bool, string>(true, string.Empty);
            //插入失败
            else
            {
                return new Tuple<bool, string>(false, errorReason);
            }

        }
        //失物招领基础信息表填写
        private Tuple<bool, string> PublistFoundItemBasic(Found_Item item)
        {
            var Names = new List<string>
            {
                "Item_ID",
                "Item_Name",
                "Category_ID",
                "Description",
                "Found_Location",
                "Found_Date",
                "User_ID",
                "Match_Status",
               " Review_Status",
            };
            var values = new List<object>
            {
                item.Item_ID,
                item.Item_Name,
                item.Category_ID,
                item.Description,
                item.Found_Location,
                item.Found_Date,
                item.User_ID,
                item.Match_Status,
                item.Review_Status
            };
            BasicSQLOps basic = new BasicSQLOps(conn);
            var result = basic.InsertOperation("Found_Item", Names, values);
            bool isSuccess = result.Item1; // 获取是否成功插入
            string errorReason = result.Item2; // 获取出错误原因
            if (isSuccess)
                return new Tuple<bool, string>(true, string.Empty);
            //插入失败
            else
            {
                return new Tuple<bool, string>(false, errorReason);
            }

        }

        //外部接口函数
        public Tuple<bool, string> PublishLostItem(List<Lost_Item> lostItems, List<Reward_Offers> rewardOffers, bool reward_or_not)
        {
            int n = 0;
            try{
                foreach (Lost_Item item in lostItems) {
                //先插入基础表单
                    var basicExcel = PublishLostItemBasic(item);
                    bool isSuccess1 = basicExcel.Item1; // 获取是否成功插入
                    string errorReason1 = "表单写入数据库过程中" + basicExcel.Item2; // 获取出错误原因
                    if (isSuccess1)
                    {
                        //图片插入成功，插入是否悬赏
                        if (!reward_or_not)
                        {
                            n++;
                            continue;
                        }
                        //有悬赏
                        else
                        {
                            var reward = HaveReward(rewardOffers[n]);
                            bool isSuccess3 = reward.Item1; // 获取是否成功插入
                            string errorReason3 = "悬赏设置过程中" + reward.Item2; // 获取出错误原因
                            if (isSuccess3)
                            {
                                n++;
                                continue;
                            }
                            //悬赏设置失败
                            else
                            { return new Tuple<bool, string>(false, errorReason3); }
                        }

                    }
                    else //插入基础表单失败
                    { 
                        return new Tuple<bool, string>(false, errorReason1); 
                    }
                }//end of foreach
                return new Tuple<bool, string>(true, string.Empty); 
            }
            catch (Exception ex)
            {
                //其他未处理的异常
                return new Tuple<bool, string>(false, "发布寻物启事发生异常: " + ex.Message);
            }
        }

        public Tuple<bool, string> PublishFoundItem(List<Found_Item> foundItems)
        {
            int n = 0;
            try
            {
                foreach (Found_Item item in foundItems)
                {
                    //先插入基础表单
                    var basicExcel = PublistFoundItemBasic(item);
                    bool isSuccess1 = basicExcel.Item1; // 获取是否成功插入
                    string errorReason1 = "表单写入数据库过程中" + basicExcel.Item2; // 获取出错误原因
                    if (isSuccess1)
                    {
                        n++;
                        continue;
                    }
                    //插入基础表单失败
                    else
                    { 
                        return new Tuple<bool, string>(false, errorReason1); 
                    }
                }
                return new Tuple<bool, string>(true, string.Empty);
            }
            catch (Exception ex)
            {
                //其他未处理的异常
                return new Tuple<bool, string>(false, "发布失物招领发生异常: " + ex.Message);
            }
        }
        public Tuple<bool, string> DeleteItem(int type, Dictionary<string, object> index)
        {
            string TableName = "";
            if (index == null)
            {
                string errorReason = "删除索引不能为空！";
                return new Tuple<bool, string>(false, errorReason);
            }
            
            else
            {
                if(type == 0)
                {TableName = "Lost_Item";}
                else if (type == 1)
                {TableName = "Found_Item";}
                else
                {
                    string errorReason = "不合法的type值，请输入0/1！";
                    return new Tuple<bool, string>(false, errorReason); 
                }
                string ErrorReason = string.Empty;
                if (OracleConnection.State == ConnectionState.Open)
                {
                    string ConditionString = string.Join(" AND ", index.Keys.Select(key => $"{key.ToUpper()} = :{key.ToUpper()}"));
                    string DeleteSQL = $"DELETE FROM {TableName.ToUpper()} WHERE {ConditionString}";

                    using (OracleCommand cmd = new OracleCommand(DeleteSQL, OracleConnection))
                    {
                        try
                        {
                            // 添加参数
                            foreach (var condition in index)
                            {
                                cmd.Parameters.Add(new OracleParameter(condition.Key.ToUpper(), condition.Value ?? DBNull.Value));
                            }
                            int AffectedRow = cmd.ExecuteNonQuery();
                            Debug.WriteLine($"共{AffectedRow}行被删除");
                            if (AffectedRow == 0)
                            {
                                return new Tuple<bool, string>(false, "删除无效");
                            }
                            return new Tuple<bool, string>(true, ErrorReason);
                        }
                        catch (Exception ex)
                        {
                            ErrorReason = ex.Message;
                            Debug.Write($"删除失败,报错为：{ErrorReason}");
                            return new Tuple<bool, string>(false, ErrorReason);
                        }
                    }
                }
                else
                {
                    ErrorReason = "数据库未连接";
                    Debug.WriteLine("删除操作，", ErrorReason);
                    return new Tuple<bool, string>(false, ErrorReason);
                }
            }
        }
        public Tuple<bool, string> QueryItem(int type, Dictionary<string, object> index)
        {
            string TableName = "";
            if (index == null)
            {
                string errorReason = "查询索引不能为空！";
                return new Tuple<bool, string>(false, errorReason);
            }
            else
            {
                if (type == 0)
                { TableName = "Lost_Items"; }
                else if (type == 1)
                { TableName = "Found_Items"; }
                else
                {
                    string errorReason = "不合法的type值，请输入0/1！";
                    return new Tuple<bool, string>(false, errorReason);
                }
                string ErrorReason = string.Empty;
                if (OracleConnection.State == ConnectionState.Open)
                {
                    string ConditionString = string.Join(" AND ", index.Keys.Select(key => $"{key.ToUpper()} = :{key.ToUpper()}"));
                    string QuerySQL = $"SELECT * FROM {TableName.ToUpper()} NATURAL JOIN REWARD_OFFERS WHERE {ConditionString}";

                    using (OracleCommand cmd = new OracleCommand(QuerySQL, OracleConnection))
                    {
                        foreach (var condition in index)
                        {
                            cmd.Parameters.Add(new OracleParameter(condition.Key.ToUpper(), condition.Value ?? DBNull.Value));
                        }

                        try
                        {
                            using (OracleDataReader reader = cmd.ExecuteReader())
                            {
                                var results = new List<Dictionary<string, object>>();
                                while (reader.Read())
                                {
                                    var row = new Dictionary<string, object>();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                    }
                                    results.Add(row);
                                }
                                return new Tuple<bool, string>(true, JsonConvert.SerializeObject(results));
                            }
                        }

                        catch (Exception ex)
                        {
                            ErrorReason = ex.Message;
                            Debug.Write($"查找失败,报错为：{ErrorReason}");
                            return new Tuple<bool, string>(false, ErrorReason);
                        }
                    }
                }
                else
                {
                    ErrorReason = "数据库未连接";
                    Debug.WriteLine("查找操作，", ErrorReason);
                    return new Tuple<bool, string>(false, ErrorReason);
                }
            }
        }


    }

    
}
