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
        private UserActivity userActivityService;

        public UserActivityController(Connection connection)
        {
            userActivityService = new UserActivity(connection);
        }

        // 查看近期活跃 测试成功 空则返回空列表
        [HttpPost("recent")]
        public ActionResult GetRecentActivities([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                // 检查是否包含 user_id 参数
                if (!request.ContainsKey("current_user_id"))
                {
                    return BadRequest(new { status = "error", message = "缺少 user_id 参数" });
                }

                var userId = request["current_user_id"].GetInt32();
                var activities = userActivityService.ViewRecentActivities(userId);

                // 格式化响应
                var response = new
                {
                    status = "success",
                    activities = activities.Select(activity => new
                    {
                        id = activity.Activity_ID,
                        type = activity.Activity_Type,
                        score = activity.Score,
                        time = activity.DateTime
                    }).ToList()
                };

                return Ok(response);

            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = $" {ex.Message}" });
            }
        }

        // 管理员新增活跃行为 测试成功
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
                string activityType = ControllerHelper.GetSafeString(request, "activity_type");
                var score = request["score"].GetInt32();
                var datetime = request["datetime"].GetDateTime();

                int activityId = userActivityService.AddActivity(userId, activityType, score, datetime);
                return Ok(new { status = "success", activity_id = activityId });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = $" {ex.Message}" });
            }
        }

        // 管理员修改活跃行为 不能修改不存在的活跃行为 测试成功
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
                string activityType = ControllerHelper.GetSafeString(request, "activity_type");
                var score = request["score"].GetInt32();
                var datetime = request["datetime"].GetDateTime();

                userActivityService.UpdateActivity(activityId, activityType, score, datetime);
                return Ok(new { status = "success" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = $"{ex.Message}" });
            }
        }

        // 管理员删除活跃行为 测试成功

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
                return BadRequest(new { status = "error", message = $"{ex.Message}" });
            }
        }

        [HttpPost("AutoAdd")]
        public IActionResult AddUserActivity([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                // 从 Dictionary 中提取数据
                if (!request.TryGetValue("user_id", out JsonElement userIdElement) ||
                    !request.TryGetValue("activity_type", out JsonElement activityTypeElement) ||
                    !request.TryGetValue("datetime", out JsonElement datetimeElement))
                {
                    return BadRequest("请求中缺少必要的字段。");
                }

                int userId = userIdElement.GetInt32();
                string activityType = ControllerHelper.GetSafeString(request, "activity_type");
                DateTime datetime = datetimeElement.GetDateTime();

                // 调用已有的 AddUserActivity 方法
                int result = userActivityService.AddUserActivity(userId, activityType, datetime);

                return Ok(new {status="success"});
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = $"{ex.Message}" });
            }
        }




    }
}
