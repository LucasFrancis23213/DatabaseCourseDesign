using DatabaseProject.ServiceLayer.ConmmunityFeature;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SQLOperation.PublicAccess.Utilities;
using System.Security.Cryptography.Pkcs;

namespace WebApplication1.APILayer.CommunityFeatureAPI
{
    [Route("api/")]
    [ApiController]
    public class UserPostsController : ControllerBase
    {
        private UserPosts userPosts;

        public UserPostsController(UserPosts u_p)
        {
            userPosts = u_p;
        }

        // 获取特定人的状态
        [HttpPost("user/{target_id}/activities")]
        public IActionResult GetUserActivities([FromBody] Dictionary<string, object> requestData)
        {
            // 检查请求中是否包含必要的参数 target_id
            if (requestData == null || !requestData.ContainsKey("target_id"))
            {
                return BadRequest("Missing required parameter: target_id");
            }

            int userId = Convert.ToInt32( requestData["target_id"]);

            try
            {
                var user_posts = userPosts.GetUserPostsByUserId(userId);
                user_posts = user_posts ?? new List<User_Posts>();

                // 构建响应格式
                var response = new
                {
                    status = "success",
                    user = new
                    {
                        id = userId.ToString(), // 假设用户信息从其他地方获取
                        name = "User Name", // 假设用户信息从其他地方获取
                        avatar = "https://example.com/avatar.jpg" // 假设用户信息从其他地方获取
                    },
                    activities = user_posts.Select(post => new
                    {
                        activity = new
                        {
                            id = post.Post_ID.ToString(),
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 获取推荐动态
        [HttpPost("user/recommendations")]
        public IActionResult GetRecommendations()
        {
            try
            {
                var recommendedPosts = userPosts.GetAllPostsSortedByPopularity();
                recommendedPosts = recommendedPosts ?? new List<User_Posts>();

                // 构建推荐动态的响应格式
                var response = new
                {
                    status = "success",
                    activities = recommendedPosts.Select(post => new
                    {
                        id = post.Post_ID.ToString(),
                        content = post.Content,
                        time = post.Post_Date,
                        heat = post.Popularity,
                        user = new
                        {
                            id = post.User_ID.ToString(), // 假设用户信息从其他地方获取
                            name = "User Name", // 假设用户信息从其他地方获取
                            avatar = "https://example.com/avatar.jpg" // 假设用户信息从其他地方获取
                        }
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 用户发布动态
        [HttpPost("posts/create")]
        public IActionResult CreatePost([FromBody] Dictionary<string, object> requestData)
        {

            // 检查请求中是否包含必要的参数 user_id, content 和 current_user_id
            if (requestData == null || !requestData.ContainsKey("user_id") || !requestData.ContainsKey("content") || !requestData.ContainsKey("current_user_id"))
            {
                return BadRequest("Invalid request body. 'user_id', 'content', and 'current_user_id' are required.");
            }

            try
            {
                // 直接转换 user_id 和 current_user_id
                int userId = Convert.ToInt32(requestData["user_id"]);
                int currentUserId = Convert.ToInt32(requestData["current_user_id"]);

                string content = requestData["content"].ToString();
                DateTime postTime = DateTime.Now;

                // 调用 UserPosts 类的 CreatePost 方法发布动态
                int postId = userPosts.CreatePost(userId, content, postTime);

                // 构建响应对象
                var response = new
                {
                    status = "success",
                    post_id = postId.ToString()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // POST: api/posts/forward
        // 用户转发动态
        [HttpPost("posts/forward")]
        public IActionResult ForwardPost([FromBody] Dictionary<string, object> requestData)
        {
            try
            {
                // 检查请求中是否包含必要的参数 post_id 和 current_user_id
                if (requestData == null || !requestData.ContainsKey("post_id") || !requestData.ContainsKey("current_user_id"))
                {
                    return BadRequest(new { status = "error", message = "Invalid request body format. 'post_id' and 'current_user_id' are required." });
                }

                // 转换 post_id 和 current_user_id
                int postId = Convert.ToInt32(requestData["post_id"]);
                int userId = Convert.ToInt32(requestData["current_user_id"]);

                // 获取要转发的动态内容
                var originalPost = userPosts.GetPostById(postId);

                if (originalPost == null)
                {
                    return NotFound(new { status = "error", message = "Post not found." });
                }

                // 创建转发的新动态
                string content = originalPost.Content; // 内容和原始动态相同
                DateTime postTime = DateTime.Now;

                int newPostId = userPosts.CreatePost(userId, content, postTime);

                if (newPostId > 0)
                {
                    return Ok(new { status = "success" });
                }
                else
                {
                    return BadRequest(new { status = "failed", message = "Failed to forward post." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = ex.Message });
            }
        }

        // 用户删除动态
        [HttpPost("posts/delete")]
        public IActionResult DeletePost([FromBody] Dictionary<string, object> requestData)
        {
            // 检查请求中是否包含必要的参数 post_id 和 current_user_id
            if (requestData == null || !requestData.ContainsKey("post_id") || !requestData.ContainsKey("current_user_id"))
            {
                return BadRequest("Invalid request body. 'post_id' and 'current_user_id' are required.");
            }

            try
            {
                // 转换 post_id 和 current_user_id
                int postId = Convert.ToInt32(requestData["post_id"]);
                int userId = Convert.ToInt32(requestData["current_user_id"]);

                
                // 调用 UserPosts 类的 DeletePost 方法删除动态
                bool deleteResult = userPosts.DeletePost(postId, userId);

                // 构建响应对象
                var response = new
                {
                    status = deleteResult ? "success" : "failure"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 用户取消关注或者关注
        [HttpPost("user/follow")]
        public IActionResult ManageFollow([FromBody] Dictionary<string, object> requestData)
        {
            // 检查请求中是否包含必要的参数 user_id, action 和 current_user_id
            if (requestData == null || !requestData.ContainsKey("user_id") || !requestData.ContainsKey("action") || !requestData.ContainsKey("current_user_id"))
            {
                return BadRequest("Invalid request body. 'user_id', 'action', and 'current_user_id' are required.");
            }

            try
            {
                

                // 转换 user_id 和 current_user_id
                int userId = Convert.ToInt32(requestData["user_id"]);
                int currentUserId = Convert.ToInt32(requestData["current_user_id"]);
                string action = requestData["action"].ToString();

                

                // 调用 UserPosts 类的 ManageFollow 方法进行关注/取消关注操作
                int result = userPosts.ManageFollow(currentUserId, userId, action, currentUserId);

                // 构建响应对象
                var response = new
                {
                    status = result > 0 ? "success" : "failure"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 查询特定用户的关注列表
        [HttpPost("user/following")]
        public IActionResult GetFollowingList([FromBody] Dictionary<string, object> requestData)
        {
            if (requestData == null || !requestData.ContainsKey("user_id"))
            {
                return BadRequest("Invalid request body. 'user_id' is required.");
            }

            try
            {
                if (!int.TryParse(requestData["user_id"].ToString(), out int userId))
                {
                    return BadRequest("Invalid 'user_id' format. Integer value expected.");
                }

                // 查询特定用户的关注列表
                var followList = userPosts.GetFollowListByUserId(userId);

                // 构建响应对象 需要额外查询
                var response = new
                {
                    status = "success",
                    following = followList.Select(follow => new
                    {
                        user_id = follow.Followed_User_ID.ToString(),
                        user_name = "",
                        user_avatar = ""
                    })
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}


