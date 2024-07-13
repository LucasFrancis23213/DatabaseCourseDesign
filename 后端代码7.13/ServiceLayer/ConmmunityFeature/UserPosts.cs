using DatabaseProject.BusinessLogicLayer.CommunityFeatureBLL;
using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Utilities;

namespace DatabaseProject.ServiceLayer.ConmmunityFeature
{
    public class UserPosts
    {
        private CommunityFeatureBusiness<User_Posts> UserPostsBusiness;
        private CommunityFeatureBusiness<Follow_List> FollowListBusiness;

        private List<string> userPostsList = new List<string> { "user_id", "content", "post_date", "popularity" };
        private List<string> followList = new List<string> { "follower_user_id", "followed_user_id", "follow_date" };

        // 构造函数
        public UserPosts(Connection connection)
        {
            UserPostsBusiness = new CommunityFeatureBusiness<User_Posts>(connection);
            FollowListBusiness = new CommunityFeatureBusiness<Follow_List>(connection);
        }

        // 根据用户id查询用户动态
        public List<User_Posts> GetUserPostsByUserId(int userId)
        {
            try
            {
                var result = UserPostsBusiness.QueryBusiness(new Dictionary<string, object> { { "user_id", userId } }, "AND");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching posts for user ID {userId}: {ex.Message}");
                throw new Exception($"Error fetching posts for user ID {userId}: {ex.Message}");
            }
        }


        // 获取所有动态 根据热度从高到低排序 待定
        public List<User_Posts> GetAllPostsSortedByPopularity()
        {
            try
            {
                var allPosts = UserPostsBusiness.QueryBusiness(new Dictionary<string, object> { { "true", "true" } },"AND");
                if (allPosts != null)
                {
                    var sortedPosts = allPosts.OrderByDescending(post => post.Popularity).ToList();
                    return sortedPosts;
                }
                else
                {
                    return new List<User_Posts>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching and sorting posts by popularity: {ex.Message}");
                throw new Exception($"Error fetching and sorting posts by popularity: {ex.Message}");
            }
        }


        // 根据动态ID查询动态
       
        public User_Posts GetPostById(int postId)
        {
            try
            {
                var result = UserPostsBusiness.QueryBusiness(new Dictionary<string, object> { { "post_id", postId } },"AND").FirstOrDefault();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching post with ID {postId}: {ex.Message}");
                throw new Exception($"Error fetching post with ID {postId}: {ex.Message}");
            }
        }



        // 根据动态内容发布动态
        public int CreatePost(int userId, string content, DateTime postTime)
        {
            try
            {
                var newPost = UserPostsBusiness.PackageData(0, userId, content, postTime, 0);
                var result = UserPostsBusiness.AddBusiness(userPostsList, "post_id", newPost);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating post for user ID {userId}: {ex.Message}");
                throw new Exception($"Error creating post for user ID {userId}: {ex.Message}");
            }
        }

        // 删除动态 顺便判断是否能删除
        public bool DeletePost(int postId, int userId)
        {
            try
            {
                var result = UserPostsBusiness.RemoveBusiness(new Dictionary<string, object> { { "post_id", postId }, { "user_id", userId } });
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting post with ID {postId}: {ex.Message}");
                throw new Exception($"Error deleting post with ID {postId}: {ex.Message}");
            }
        }


        // 根据要取消关注的用户ID和操作类型进行关注或者取关某用户
        public int ManageFollow(int userId, int targetUserId, string actionType, int localId)
        {
            try
            {
                if (actionType == "follow")
                {
                    var followEntry = FollowListBusiness.PackageData(0, userId, targetUserId, DateTime.Now);
                    var result = FollowListBusiness.AddBusiness(followList, "follow_id", followEntry);
                    return result;
                }
                else if (actionType == "unfollow")
                {
                    var result = FollowListBusiness.RemoveBusiness(new Dictionary<string, object> { { "follower_id", localId }, { "followed_id", userId } });
                    return Convert.ToInt32(result);
                }
                return 0; // Invalid action type
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error managing follow action: {ex.Message}");
                throw new Exception($"Error managing follow action: {ex.Message}");
            }
        }



        // 查询特定用户关注列表 根据要查询的用户ID
        public List<Follow_List> GetFollowListByUserId(int userId)
        {
            try
            {
                var result = FollowListBusiness.QueryBusiness(new Dictionary<string, object> { { "follower_user_id", userId } }, "AND" );
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching follow list for user ID {userId}: {ex.Message}");
                throw new Exception($"Error fetching follow list for user ID {userId}: {ex.Message}");
            }
        }


    }
}
