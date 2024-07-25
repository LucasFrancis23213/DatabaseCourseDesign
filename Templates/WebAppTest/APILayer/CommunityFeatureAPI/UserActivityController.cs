using DatabaseProject.ServiceLayer.ConmmunityFeature;
using Microsoft.AspNetCore.Mvc;
using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Utilities;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Transactions;
using DatabaseProject.BusinessLogicLayer.ServiceLayer.ConmmunityFeature;

namespace WebAppTest.APILayer.CommunityFeatureAPI
{
    [ApiController]
    [Route("api/activity/")]
    public class UserActivityController : Controller
    {
        private readonly Connection connection;
        private UserActivity userActivityService;

        public UserActivityController(Connection connection)
        {
            userActivityService = new UserActivity(connection);
        }

        // 查看近期活跃
        [HttpPost("recent")]
        public ActionResult GetRecentActivities([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                // 检查是否包含 user_id 参数
                if (!request.ContainsKey("user_id"))
                {
                    return BadRequest(new { status = "error", message = "缺少 user_id 参数" });
                }

                var userId = request["user_id"].GetInt32();
                var activities = userActivityService.ViewRecentActivities(userId);

                // 格式化响应
                var response = new
                {
                    status = "success",
                    activities = activities.Select(activity => new
                    {
                        activity_id = activity.Activity_ID,
                        activity_type = activity.Activity_Type,
                        score = activity.Score,
                        datetime = activity.DateTime
                    }).ToList()
                };

                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = $"获取近期活跃行为时发生错误: {ex.Message}" });
            }
        }

        // 管理员新增活跃行为
        [HttpPost("add")]
        public ActionResult AddActivity([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                // 检查是否包含所有必要的参数
                if (!request.ContainsKey("user_id") || !request.ContainsKey("activity_type") ||
                    !request.ContainsKey("score") || !request.ContainsKey("datetime"))
                {
                    return BadRequest(new { status = "error", message = "缺少必要参数" });
                }

                var userId = request["user_id"].GetInt32();
                var activityType = request["activity_type"].GetString();
                var score = request["score"].GetInt32();
                var datetime = request["datetime"].GetDateTime();

                int activityId = userActivityService.AddActivity(userId, activityType, score, datetime);
                return Ok(new { status = "success", activity_id = activityId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = $"新增活跃行为时发生错误: {ex.Message}" });
            }
        }

        // 管理员修改活跃行为
        [HttpPut("update")]
        public ActionResult UpdateActivity([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                // 检查是否包含所有必要的参数
                if (!request.ContainsKey("activity_id") || !request.ContainsKey("activity_type") ||
                    !request.ContainsKey("score") || !request.ContainsKey("datetime"))
                {
                    return BadRequest(new { status = "error", message = "缺少必要参数" });
                }

                var activityId = request["activity_id"].GetInt32();
                var activityType = request["activity_type"].GetString();
                var score = request["score"].GetInt32();
                var datetime = request["datetime"].GetDateTime();

                userActivityService.UpdateActivity(activityId, activityType, score, datetime);
                return Ok(new { status = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = $"修改活跃行为时发生错误: {ex.Message}" });
            }
        }

        // 管理员删除活跃行为

        [HttpDelete("delete")]
        public ActionResult DeleteActivity([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                // 检查是否包含 activity_id 参数
                if (!request.ContainsKey("activity_id"))
                {
                    return BadRequest(new { status = "error", message = "缺少 activity_id 参数" });
                }

                var activityId = request["activity_id"].GetInt32();
                userActivityService.DeleteActivity(activityId);
                return Ok(new { status = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = $"删除活跃行为时发生错误: {ex.Message}" });
            }
        }



    }
}
