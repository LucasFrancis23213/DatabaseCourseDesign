using DatabaseProject.ServiceLayer.ConmmunityFeature;
using Microsoft.AspNetCore.Mvc;
using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Utilities;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Transactions;
using WebAppTest.APILayer.CommunityFeatureAPI;

namespace WebApplication1.APILayer.CommunityFeatureAPI
{
    [Route("api/")]
    [ApiController]
    public class UserPostsController : ControllerBase
    {
        private UserPosts userPosts;

        public UserPostsController(Connection connection)
        {
            userPosts = new UserPosts(connection);
        }

        // 获取特定人的动态
        [HttpPost("user/{target_id}/activities")]
        public IActionResult GetUserActivities(int target_id)
        {
            // 检查请求中是否包含必要的参数 target_id
            

            int userId = Convert.ToInt32( target_id);

            try
            {
                var user=userPosts.GetUserByID(userId);
                if (user == null)
                {
                    return StatusCode(500, $"未找到指定的用户");
                }

                var user_posts = userPosts.GetUserPostsByUserId(userId);

                
                // 构建响应格式
                var response = new
                {
                    status = "success",
                    user = new
                    {
                        id = userId, // 假设用户信息从其他地方获取
                        name = user.User_Name, // 假设用户信息从其他地方获取
                        avatar = "https://example.com/avatar.jpg" // 假设用户信息从其他地方获取
                    },
                    activities = user_posts.Select(post => new
                    {
                        activity = new
                        {
                            id = post.Post_ID,
                            content = post.Content,
                            time = post.Post_Date,
                            heat = post.Popularity
                        }
                    })
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"内部服务器错误: {ex.Message}");
            }
        }

        // 获取推荐动态
        [HttpPost("user/recommendations")]
        public IActionResult GetRecommendations()
        {
            try
            {
                var recommendedPosts = userPosts.GetAllPostsSortedByPopularity();
                

                // 构建推荐动态的响应格式
                var response = new
                {
                    status = "success",
                    activities = recommendedPosts.Select(post => new
                    {
                        id = post.Item2.Post_ID,
                        content = post.Item2.Content,
                        time = post.Item2.Post_Date,
                        heat = post.Item2.Popularity,
                        user = new
                        {
                            id = post.Item1.User_ID, // 假设用户信息从其他地方获取
                            name = post.Item1.User_Name, // 假设用户信息从其他地方获取
                            avatar = "https://example.com/avatar.jpg" // 假设用户信息从其他地方获取
                        }
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"内部服务器错误: {ex.Message}");
            }
        }

        // 用户发布动态 使用外键限制不存在的用户
        [HttpPost("posts/create")]
        public IActionResult CreatePost([FromBody] Dictionary<string, JsonElement> requestData)
        {

            // 检查请求中是否包含必要的参数 user_id, content 和 current_user_id
            if (requestData == null || !requestData.ContainsKey("user_id") || !requestData.ContainsKey("content") || !requestData.ContainsKey("time"))
            {
                return BadRequest("请求体无效。必须包含 'user_id'、'content' 和 'current_user_id'。");

            }

            try
            {
                // 直接转换 user_id 和 current_user_id
                int userId = requestData["user_id"].GetInt32();
                string content = ControllerHelper.GetSafeString(requestData, "content");
                DateTime postTime = requestData["time"].GetDateTime();

                // 调用 UserPosts 类的 CreatePost 方法发布动态
                int postId = userPosts.CreatePost(userId, content, postTime);

                // 构建响应对象
                var response = new
                {
                    status = "success",
                    post_id = postId
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"内部服务器错误: {ex.Message}");

            }
        }

        // POST: api/posts/forward
        // 用户转发动态 限制转发对象 
        [HttpPost("posts/forward")]
        public IActionResult ForwardPost([FromBody] Dictionary<string, JsonElement> requestData)
        {
            try
            {
                // 检查请求中是否包含必要的参数 post_id 和 current_user_id
                if (requestData == null || !requestData.ContainsKey("post_id") || !requestData.ContainsKey("current_user_id")||!requestData.ContainsKey("time"))
                {
                    return BadRequest(new { status = "error", message = "请求体格式无效。必须包含 'post_id' 和 'current_user_id'。" });

                }

                // 转换 post_id 和 current_user_id
                int postId = requestData["post_id"].GetInt32();
                int userId = requestData["current_user_id"].GetInt32();
                DateTime postTime = requestData["time"].GetDateTime();

                // 获取要转发的动态内容
                var originalPost = userPosts.GetPostById(postId);

                if (originalPost == null)
                {
                    return NotFound(new { status = "error", message = "未找到帖子。" });

                }

                // 创建转发的新动态
                string content = originalPost.Content; // 内容和原始动态相同
                

                int newPostId = userPosts.CreatePost(userId, content, postTime);

                if (newPostId > 0)
                {
                    return Ok(new { status = "success" });
                }
                else
                {
                    return BadRequest(new { status = "failed", message = "转发失败" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = ex.Message });
            }
        }

        // 用户删除动态
        [HttpPost("posts/delete")]
        public IActionResult DeletePost([FromBody] Dictionary<string, JsonElement> requestData)
        {
            // 检查请求中是否包含必要的参数 post_id 和 current_user_id
            if (requestData == null || !requestData.ContainsKey("post_id") || !requestData.ContainsKey("current_user_id"))
            {
                return BadRequest("请求体无效。必须包含 'post_id' 和 'current_user_id'。");

            }

            try
            {
                // 转换 post_id 和 current_user_id
                int postId = requestData["post_id"].GetInt32();
                int userId = requestData["current_user_id"].GetInt32();

                
                // 调用 UserPosts 类的 DeletePost 方法删除动态
                bool deleteResult = userPosts.DeletePost(postId, userId);

                // 构建响应对象
                var response = new
                {
                    status = deleteResult ? "success" : "fail"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"内部服务器错误: {ex.Message}");
            }
        }

        // 用户取消关注或者关注 限制唯一性
        [HttpPost("user/follow")]
        public IActionResult ManageFollow([FromBody] Dictionary<string, JsonElement> requestData)
        {
            // 检查请求中是否包含必要的参数 user_id, action 和 current_user_id
            if (requestData == null || !requestData.ContainsKey("user_id") || !requestData.ContainsKey("action") || !requestData.ContainsKey("current_user_id"))
            {
                return BadRequest("请求体无效。必须包含 'user_id'、'action' 和 'current_user_id'。");

            }

            try
            {
                
                // 转换 user_id 和 current_user_id
                int userId = requestData["user_id"].GetInt32();
                int currentUserId = requestData["current_user_id"].GetInt32();
                string action = ControllerHelper.GetSafeString(requestData, "action");



                // 调用 UserPosts 类的 ManageFollow 方法进行关注/取消关注操作
                int result = userPosts.ManageFollow(currentUserId, userId, action);

                // 构建响应对象
                var response = new
                {
                    status = result > 0 ? "success" : "error"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = $"内部服务器错误:{ex.Message}" });
            }
        }

        // 查询特定用户的关注列表
        [HttpPost("user/following")]
        public IActionResult GetFollowingList([FromBody] Dictionary<string, JsonElement> requestData)
        {
            if (requestData == null || !requestData.ContainsKey("user_id"))
            {
                return BadRequest("请求体无效。必须包含 'user_id'。");

            }

            try
            {
                int userId = requestData["user_id"].GetInt32();

                // 查询特定用户的关注列表
                var followList = userPosts.GetFollowListByUserId(userId);

                // 构建响应对象 需要额外查询
                var response = new
                {
                    status = "success",
                    following = followList.Select(follow => new
                    {
                        user_id = follow.Item1.Followed_User_ID,
                        user_name = follow.Item2.User_Name,
                        user_avatar = ""
                    })
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status="error", message= $"内部服务器错误: {ex.Message}" } );

            }
        }

        // 查询对指定用户的关注状态 
        // 获取当前用户对特定用户的关注状态 待定
        [HttpPost("user/follow/status")]
        public IActionResult GetFollowStatus([FromBody] Dictionary<string, JsonElement> requestData)
        {
            // 检查请求中是否包含必要的参数 target_id 和 current_user_id
            if (requestData == null || !requestData.ContainsKey("target_id") || !requestData.ContainsKey("current_user_id"))
            {
                return BadRequest("请求体无效。必须包含 'target_id' 和 'current_user_id'。");
            }

            try
            {
                // 转换参数
                int targetUserId = requestData["target_id"].GetInt32();
                int currentUserId = requestData["current_user_id"].GetInt32();

                var result=userPosts.CheckFollowStatus(currentUserId,targetUserId);

                // 构建响应对象
                var response = new
                {
                    status = "success",
                    is_following = result
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = $"内部服务器错误: {ex.Message}" });
            }
        }

        // 查询特定用户的粉丝列表
        [HttpPost("user/followers")]
        public IActionResult GetFollowersList([FromBody] Dictionary<string, JsonElement> requestData)
        {
            // 检查请求中是否包含必要的参数 user_id
            if (requestData == null || !requestData.ContainsKey("user_id"))
            {
                return BadRequest("请求体无效。必须包含 'user_id'。");
            }

            try
            {
                // 转换参数
                int userId= requestData["user_id"].GetInt32();
                

                // 查询特定用户的粉丝列表
                var followersList = userPosts.GetFollowersByUserId(userId);

                // 构建响应对象
                var response = new
                {
                    status = "success",
                    followers = followersList.Select(follower => new
                    {
                        user_id = follower.Item1.Follower_User_ID,
                        user_name = follower.Item2.User_Name,
                        user_avatar = "" // 如果有用户头像的字段，需要从数据库中获取
                    })
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { status = "error", message = $"内部服务器错误: {ex.Message}" });
            }
        }




    }
}


