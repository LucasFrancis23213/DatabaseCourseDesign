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
    [Route("api/comments/")]
    public class CommentsController : Controller
    {
        private Comments commentService;
        private UserActivity userActivity;
        private Connection CommentsConnection;

        public CommentsController(Connection connection)
        {
            commentService=new Comments(connection);
            userActivity=new UserActivity(connection);
            CommentsConnection = connection;
        }

        // 修改 不要重复报错
        // 添加
        [HttpPost("add")]
        public ActionResult AddComment([FromBody] Dictionary<string, JsonElement> request)
        {
            using (var transaction = CommentsConnection.GetOracleConnection().BeginTransaction())
            {
                try
                {
                    // 检查是否包含所有必要的参数
                    if (!request.ContainsKey("item_id") || !request.ContainsKey("user_id") ||
                        !request.ContainsKey("content") || !request.ContainsKey("time"))
                    {
                        return BadRequest(new { status = "error", message = "缺少必要参数" });
                    }

                    // 从 Dictionary 中提取参数
                    string itemId = ControllerHelper.GetSafeString(request, "item_id");
                    var userId = request["user_id"].GetInt32();
                    string content = ControllerHelper.GetSafeString(request, "content");
                    DateTime time = request["time"].GetDateTime();

                    // 执行评论发布和用户活动记录
                    int commentId = commentService.PostComment(itemId, userId, content, time);
                    userActivity.AddUserActivity(userId, "评论", time);

                    transaction.Commit();
                    return Ok(new { status = "success", comment_id = commentId });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return BadRequest(new { status = "error", message = $"{ex.Message}" });
                }

            }


        }

        // 删除
        [HttpDelete("delete")]
        public ActionResult DeleteComment([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                // 检查是否包含所有必要的参数
                if (!request.ContainsKey("comment_id") || !request.ContainsKey("user_id"))
                {
                    return BadRequest(new { status = "error", message = "缺少必要参数" });
                }

                var commentId = request["comment_id"].GetInt32();
                var userId = request["user_id"].GetInt32();

                bool success = commentService.DeleteComment(commentId, userId);
                if (success)
                {
                    return Ok(new { status = "success" });
                }
                else
                {
                    return BadRequest(new { status = "error", message = "删除评论失败" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = $"{ex.Message}" });
            }
        }

        // 查看
        [HttpPost("item")]
        public ActionResult ViewItemComments([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                // 检查是否包含所有必要的参数
                if (!request.ContainsKey("item_id"))
                {
                    return BadRequest(new { status = "error", message = "缺少必要参数" });
                }

                string itemId = ControllerHelper.GetSafeString(request, "item_id");

                var commentsList = commentService.ViewItemComments(itemId);
                // 格式化响应
                var response = new
                {
                    status = "success",
                    comments = commentsList.Select(comment => new
                    {
                        id = comment.Item2.Comment_ID,

                        user = new
                        {
                            id = comment.Item2.User_ID,
                            name = comment.Item1.User_Name,
                            avatar = ""
                        },
                        content = comment.Item2.Content,
                        time = comment.Item2.Datetime.ToLocalTime()
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = $"{ex.Message}" });
            }
        }
    }
}
