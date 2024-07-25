using DatabaseProject.BusinessLogicLayer.CommunityFeatureBLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Templates.SQLManager;
using Oracle.ManagedDataAccess.Client;

namespace DatabaseProject.BusinessLogicLayer.ServiceLayer.ConmmunityFeature
{
    public class UserActivity
    {
        private CommunityFeatureBusiness<Users> UsersBusiness;
        private CommunityFeatureBusiness<User_Activity> UserActivitiesBusiness;
        private CommunityFeatureBusiness<User_Points> UserPointsBusiness;

        private List<string> UserActivityList = new List<string> {  "user_id", "activity_type", "score", "datetime" };
        private List<string> UserPointsList = new List<string> { "user_id","points"};
        // 构造函数
        public UserActivity(Connection connection)
        {
            UsersBusiness = new CommunityFeatureBusiness<Users>(connection);
            UserActivitiesBusiness = new CommunityFeatureBusiness<User_Activity>(connection);
            UserPointsBusiness = new CommunityFeatureBusiness<User_Points>(connection);
        }

        // 用户查看近期活跃行为 参数为user_id 返回List<User_Activity>
        public List<User_Activity> ViewRecentActivities(int userId)
        {
            try
            {
                string whereClause = "USER_ID = :USER_ID ORDER BY DATETIME DESC";
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter(":USER_ID", userId),
                   
                };

                return UserActivitiesBusiness.QueryTableWithWhereBusiness(whereClause, parameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                throw new Exception($"{ex.Message}");
            }
        }


        //管理员新增活跃行为 参数为user_id activity_type score datetime（可以供其他组使用）
        public int AddActivity(int userId, string activityType, int score, DateTime datetime)
        {
            try
            {

                var activity = UserActivitiesBusiness.PackageData(0, userId, activityType, score, datetime);

                return UserActivitiesBusiness.AddBusiness(UserActivityList, "ACTIVITY_ID", activity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                throw new Exception($"{ex.Message}");
            }
        }

        // 管理员修改活跃行为 参数为activity_id activity_type score datetime
        public bool UpdateActivity(int activityId, string activityType, int score, DateTime datetime)
        {
            try
            {
                Dictionary<string, object> updateColumns = new Dictionary<string, object>
                    {
                        { "ACTIVITY_TYPE", activityType },
                        { "SCORE", score },
                        { "datetime", datetime }
                    };
                Dictionary<string, object> condition = new Dictionary<string, object>
                    {
                        { "ACTIVITY_ID", activityId }
                    };
                if (UserActivitiesBusiness.UpdateBusiness(updateColumns, condition) > 0)
                {
                    return true;
                }
                else
                {
                    throw new Exception("修改活跃行为时发生错误");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                throw new Exception($"{ex.Message}");
            }
        }

        //管理员删除活跃行为 参数为activity_id
        public bool DeleteActivity(int activityId)
        {
            try
            {
                Dictionary<string, object> condition = new Dictionary<string, object>
                {
                    { "ACTIVITY_ID", activityId }
                };
                return UserActivitiesBusiness.RemoveBusiness(condition);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"删除活跃行为时发生错误：{ex.Message}");
                throw new Exception($"{ex.Message}");
            }
        }

        //用户查看整体活跃度 参数为user_id 返回overall_score 没有记录则返回0
        public int ViewOverallScore(int userId)
        {
            try
            {
                return RecentActivityScores(userId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"查看整体活跃度时发生错误：{ex.Message}");
                throw new Exception($"{ex.Message}");
            }
        }

        // 管理员查看所有用户的整体活跃度 不输入 输出参数为List<User_Points>
        // 自定义select from 和where语句
        // users表和user_activity表连表查询 并且使用外连接
        public List<User_Points> ViewAllOverallScores()
        {
            try
            {
                // 计算七天前的日期
                DateTime sevenDaysAgo = DateTime.Now.AddDays(-7);

                // 自定义查询语句的各个部分
                // 定义 SQL 查询语句
                string selectClause = @"
                    u.USER_ID AS USER_ID, 
                    NVL(SUM(CASE 
                            WHEN ua.DATETIME >= :SEVEN_DAYS_AGO 
                            THEN ua.SCORE 
                            ELSE 0 
                           END), 0) AS POINTS";
                string fromClause = "USERS u LEFT OUTER JOIN USER_ACTIVITY ua ON u.USER_ID = ua.USER_ID";
                string whereClause = "1=1 GROUP BY u.USER_ID";
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter(":SEVEN_DAYS_AGO", sevenDaysAgo)
                };

                // 执行查询
                var userScores = UserActivitiesBusiness.QueryTableWithSelectBusiness(selectClause, fromClause, whereClause, parameters);

                // 映射查询结果
                List<User_Points> result = new List<User_Points>();
                foreach (var row in userScores)
                {
                    // 将数据映射到 Users 和 Questions 对象
                    User_Points userPoints = UserPointsBusiness.MapDictionaryToObject(row);

                    result.Add(userPoints);
                }

                return result;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"查看所有用户的整体活跃度时发生错误：{ex.Message}");
                throw new Exception($"{ex.Message}");
            }
        }


        // 在用户进行一些行为时新增活跃度 无效增加返回id为0（重复添加一天之内的登录 和错误添加区别开）
        public int AddUserActivity(int userId, string activityType,DateTime datetime)
        {
            try
            {
                // 根据行为类型设置评分
                int score = 0;
                switch (activityType)
                {
                    case "登录":
                        score = 1;
                        break;
                    case "评论":
                        score = 3;
                        break;
                    case "发帖":
                        score = 10;
                        break;
                    case "问答":
                        score = 5;
                        break;
                    default:
                        throw new Exception("未知的活动类型");
                }

                // 如果活动类型是登录，先检查当天是否已有相同类型的活跃行为
                if (activityType == "登录")
                {
                    Dictionary<string, object> condition = new Dictionary<string, object>
                         {
                             { "USER_ID", userId },
                             { "ACTIVITY_TYPE", activityType },
                         };

                    var existingActivities = UserActivitiesBusiness.QueryBusiness(condition, "AND");

                    // 检查是否有活动记录
                    if (existingActivities != null && existingActivities.Count > 0)
                    {
                        // 获取当天的日期部分
                        DateTime targetDate = datetime.Date;

                        // 筛选当天的条目
                        var activitiesOnTargetDate = existingActivities
                            .Where(activity => activity.DateTime.Date == targetDate)
                            .ToList();

                        if (activitiesOnTargetDate.Count > 0)
                        {
                            Console.WriteLine($"用户 {userId} 在 {targetDate} 已有登录活动，不重复添加。");
                            return 0; // 已存在，不重复添加
                        }
                    }
                }

                return AddActivity(userId, activityType, score, datetime);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"新增用户活跃度时发生错误：{ex.Message}");
                throw new Exception($"新增用户活跃度时发生错误：{ex.Message}");
            }
        }

        // 计算某用户总分数
        public int RecentActivityScores(int userId)
        {
            try
            {
                // 计算七天前的日期
                DateTime sevenDaysAgo = DateTime.Now.AddDays(-7);

                // 查询最近7天内的指定用户的活动记录
                string whereClause = "USER_ID = :USER_ID AND DATETIME >= :SEVEN_DAYS_AGO";
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter(":USER_ID", userId),
                    new OracleParameter(":SEVEN_DAYS_AGO", sevenDaysAgo)
                };

                List<User_Activity> recentActivities = UserActivitiesBusiness.QueryTableWithWhereBusiness(whereClause, parameters);    
                // 计算该用户的总分，如果recentActivities为空则总分为0
                int totalScore = recentActivities.Any() ? recentActivities.Sum(activity => activity.Score) : 0;
                return totalScore;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新用户分数时发生错误：{ex.Message}");
                throw new Exception($"{ex.Message}");
            }
        }




    }
}
