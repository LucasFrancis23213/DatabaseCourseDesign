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
    [Route("api/overallactivity/")]
    public class OverallActivityController : Controller
    {
        private UserActivity userActivityService;

        public OverallActivityController(Connection connection)
        {
            userActivityService = new UserActivity(connection);
        }

        // 5. 用户查看整体活跃度 测试成功
        [HttpPost("overall")]
        public IActionResult GetOverallActivity([FromBody] Dictionary<string, JsonElement> requestData)
        {
            try
            {
                // 检查请求中是否包含必要的参数 user_id
                if (requestData == null || !requestData.ContainsKey("current_user_id"))
                {
                    return BadRequest("缺少必需的参数：user_id");
                }

                // 使用 GetInt32 方法直接获取整数类型的值
                var userId = requestData["current_user_id"].GetInt32();

                // 调用服务层方法获取整体活跃度
                var overallScore = userActivityService.ViewOverallScore(userId);

                // 构建响应对象
                var response = new
                {
                    status = "success",
                    overall_score = overallScore
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取整体活跃度时出错：{ex.Message}");
                return BadRequest(new { status = "error", message = $"{ex.Message}" });
            }
        }


        // POST: /api/overallactivity/all
        [HttpPost("all")]
        public IActionResult GetAllUserOverallActivity()
        {
            try
            {
                // 调用服务层方法获取所有用户的整体活跃度
                var overallActivities = userActivityService.ViewAllOverallScores();

                // 构建响应对象
                var response = new
                {
                    status = "success",
                    overall_activities = overallActivities.Select(activity => new
                    {
                        user_id = activity.User_ID,
                        activity_score = activity.Points
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取所有用户整体活跃度时出错：{ex.Message}");
                return BadRequest(new { status = "error", message = $"{ex.Message}" });
            }
        }
    }
}



