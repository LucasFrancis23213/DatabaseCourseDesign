using DatabaseProject.BusinessLogicLayer.CommunityFeatureBLL;
using DatabaseProject.DataAccessLayer.CommunityFeatureDAL;
using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Utilities;

namespace DatabaseProject.ServiceLayer.ConmmunityFeature
{
    public class UserPosts
    {
        private CommunityFeatureBusiness<User_Posts> UserPostsBusiness;
        private CommunityFeatureBusiness<Follow_List> FollowListBusiness;
        private CommunityFeatureBusiness<Users> UserBusiness;

        private List<string> userPostsList = new List<string> { "user_id", "content", "post_date", "popularity" };
        private List<string> followList = new List<string> { "follower_user_id", "followed_user_id", "follow_date" };

        // 构造函数
        public UserPosts(Connection connection)
        {
            UserPostsBusiness = new CommunityFeatureBusiness<User_Posts>(connection);
            FollowListBusiness = new CommunityFeatureBusiness<Follow_List>(connection);
            UserBusiness=new CommunityFeatureBusiness<Users>(connection);
        }

        // 根据用户ID查询用户动态
        public List<User_Posts> GetUserPostsByUserId(int userId)
        {
            try
            {
                // 定义 FROM 子句
                string fromClause = "USER_POSTS";

                // 定义 WHERE 子句
                string whereClause = "USER_ID = :userId";

                // 定义参数
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter(":userId", userId)
                };

                // 调用查询方法
                List<Dictionary<string, object>> rowList = UserPostsBusiness.QueryTableWithFromAndWhereBusiness(fromClause, whereClause, parameters);

                List<User_Posts> result = new List< User_Posts>();

                foreach (var row in rowList)
                {
                    
                    User_Posts post = UserPostsBusiness.MapDictionaryToObject(row);

                    result.Add(post);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取用户 ID {userId} 的帖子时出错：{ex.Message}");
                throw new Exception($"获取用户 ID {userId} 的帖子时出错：{ex.Message}");
            }
        }


        // 获取所有动态 根据热度从高到低排序
        public List<Tuple<Users, User_Posts>> GetAllPostsSortedByPopularity()
        {
            try
            {
                // 定义 FROM 子句
                string fromClause = "USER_POSTS NATURAL JOIN USERS";

                // 定义 WHERE 子句，这里不需要 WHERE 子句，因为我们获取所有帖子
                string whereClause = "1=1";

                // 调用查询方法
                List<Dictionary<string, object>> rowList = UserPostsBusiness.QueryTableWithFromAndWhereBusiness(fromClause, whereClause, null);

                List<Tuple<Users, User_Posts>> result = new List<Tuple<Users, User_Posts>>();

                foreach (var row in rowList)
                {
                    // 将数据映射到 Users 和 User_Posts 对象
                    Users user = UserBusiness.MapDictionaryToObject(row);
                    User_Posts post = UserPostsBusiness.MapDictionaryToObject(row);

                    result.Add(new Tuple<Users, User_Posts>(user, post));
                }

                // 按照帖子的热度从高到低排序
                result = result.OrderByDescending(tuple => tuple.Item2.Popularity).ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取并按热度排序帖子时出错：{ex.Message}");
                throw new Exception($"获取并按热度排序帖子时出错：{ex.Message}");
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
                Console.WriteLine($"获取帖子 ID {postId} 时出错：{ex.Message}");
                throw new Exception($"获取帖子 ID {postId} 时出错：{ex.Message}");

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
                Console.WriteLine($"创建用户 ID {userId} 的帖子时出错：{ex.Message}");
                throw new Exception($"创建用户 ID {userId} 的帖子时出错：{ex.Message}");

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
                Console.WriteLine($"删除帖子 ID {postId} 时出错：{ex.Message}");
                throw new Exception($"删除帖子 ID {postId} 时出错：{ex.Message}");

            }
        }


        // 根据要取消关注的用户ID和操作类型进行关注或者取关某用户
        public int ManageFollow(int userId, int targetUserId, string actionType)
        {
            try
            {
                if (userId == targetUserId)
                {
                    throw new Exception("不能关注自己");
                }

                if (actionType == "follow")
                {
                    var followEntry = FollowListBusiness.PackageData(0, userId, targetUserId, DateTime.Now);
                    var result = FollowListBusiness.AddBusiness(followList, "follow_id", followEntry);
                    return result;
                }
                else if (actionType == "unfollow")
                {
                    var result = FollowListBusiness.RemoveBusiness(new Dictionary<string, object> { { "follower_user_id", userId }, { "followed_user_id", targetUserId } });
                    return Convert.ToInt32(result);
                }
                return 0; // Invalid action type
            }
            catch (Exception ex)
            {
                Console.WriteLine($"管理关注操作时出错：{ex.Message}");
                throw new Exception($"管理关注操作时出错：{ex.Message}");

            }
        }

        
        // 获取指定用户的基本信息界面
        public  bool CheckFollowStatus(int userId, int targetUserId)
        {
            try
            {
               
                string whereClause = "FOLLOWER_USER_ID = :userId AND FOLLOWED_USER_ID = :targetUserId";
                // 定义查询参数
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter(":userId", userId),
                    new OracleParameter(":targetUserId", targetUserId)
                };

                // 执行查询
                var result = FollowListBusiness.QueryTableWithWhereBusiness(whereClause,parameters);

                if(result.Count == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }

                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"检查用户 ID {userId} 是否关注用户 ID {targetUserId} 时出错：{ex.Message}");
                throw new Exception($"检查用户 ID {userId} 是否关注用户 ID {targetUserId} 时出错：{ex.Message}");
            }
        }

        // 查询特定用户的粉丝列表 根据要查询的用户ID
        public List<Tuple<Follow_List,Users>> GetFollowersByUserId(int userId)
        {
            try
            {
                // 定义 FROM 子句，连表查询 FOLLOW_LIST 和 USERS 表
                string fromClause = "FOLLOW_LIST JOIN USERS ON FOLLOWER_USER_ID = USER_ID";

                // 定义 WHERE 子句，包括查询条件
                string whereClause = "FOLLOWED_USER_ID = :userId";

                // 定义参数
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter(":userId", userId)
                };

                var rowList = FollowListBusiness.QueryTableWithFromAndWhereBusiness(fromClause, whereClause, parameters);

                // 使用列表存储结果
                List<Tuple<Follow_List, Users>> followList = new List<Tuple<Follow_List, Users>>();

                foreach (var row in rowList)
                {
                    // 将数据映射到 Follow_List 和 Users 对象
                    Follow_List follow = FollowListBusiness.MapDictionaryToObject(row);
                    Users user = UserBusiness.MapDictionaryToObject(row);

                    // 添加到结果列表
                    followList.Add(new Tuple<Follow_List, Users>(follow, user));
                }

                return followList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取用户 ID {userId} 的粉丝列表时出错：{ex.Message}");
                throw new Exception($"获取用户 ID {userId} 的粉丝列表时出错：{ex.Message}");

            }
        }


        // 查询特定用户关注列表 根据要查询的用户ID
        public List<Tuple<Follow_List,Users>> GetFollowListByUserId(int userId)
        {
            try
            {
                // 定义 FROM 子句，连表查询 FOLLOW_LIST 和 USERS 表
                string fromClause = "FOLLOW_LIST JOIN USERS ON FOLLOWED_USER_ID = USER_ID";

                // 定义 WHERE 子句，包括查询条件
                string whereClause = "FOLLOWER_USER_ID = :userId";

                // 定义参数
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter(":userId", userId)
                };

                var rowList = FollowListBusiness.QueryTableWithFromAndWhereBusiness(fromClause,whereClause,parameters);

                // 使用列表存储结果
                List<Tuple<Follow_List, Users>> followList = new List<Tuple<Follow_List, Users>>();

                foreach (var row in rowList)
                {
                    // 将数据映射到 Follow_List 和 Users 对象
                    Follow_List follow = FollowListBusiness.MapDictionaryToObject(row);
                    Users user = UserBusiness.MapDictionaryToObject(row);

                    // 添加到结果列表
                    followList.Add(new Tuple<Follow_List, Users>(follow, user));
                }

                return followList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取用户 ID {userId} 的关注列表时出错：{ex.Message}");
                throw new Exception($"获取用户 ID {userId} 的关注列表时出错：{ex.Message}");

            }
        }

        // 查询特定用户信息 根据用户ID
        public Users GetUserByID(int userId)
        {
            try
            {
                var user=UserBusiness.QueryBusiness(new Dictionary<string, object> { { "user_id",userId} },"AND");
                return user.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"获取用户 ID {userId} 的信息时出错：{ex.Message}");
                throw new Exception($"获取用户 ID {userId} 的信息时出错：{ex.Message}");
            }
        }


    }
}
