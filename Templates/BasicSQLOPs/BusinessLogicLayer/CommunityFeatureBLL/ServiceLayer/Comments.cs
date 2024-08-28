using DatabaseProject.BusinessLogicLayer.CommunityFeatureBLL;
using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseProject.BusinessLogicLayer.ServiceLayer.ConmmunityFeature
{
   
    public class Comments {
        private CommunityFeatureBusiness<Users> UserBusiness;
        private CommunityFeatureBusiness<Item_Comments> CommentBusiness;        

        private List<string> ItemCommentsList = new List<string> { "item_id", "user_id", "content", "datetime" };

        // 构造函数
        public Comments(Connection connection)
        {
            UserBusiness = new CommunityFeatureBusiness<Users>(connection);
            CommentBusiness = new CommunityFeatureBusiness<Item_Comments>(connection);
        }

        //用户发表评论 参数为item_id user_id content time
        public int PostComment(string itemId, int userId, string content, DateTime time)
        {
            try {
                var comment = CommentBusiness.PackageData(0, itemId, userId, content, time);
                int newCommentId = CommentBusiness.AddBusiness(ItemCommentsList, "comment_id", comment);
                return newCommentId;
            }
            catch(Exception ex) {
                throw new Exception($"用户发表评论时发生错误：{ex.Message}");
            }
            
        }

        // 用户删除自己的评论 参数为comment_id user_id
        public bool DeleteComment(int commentId, int userId)
        {
            try
            {
                // 确保用户只能删除自己的评论
                Dictionary<string, object> condition = new Dictionary<string, object>
                {
                    { "comment_id", commentId },
                    { "user_id", userId }
                };

                return CommentBusiness.RemoveBusiness(condition);
            }
            catch (Exception ex)
            {
                throw new Exception($"用户删除评论时发生错误：{ex.Message}");
            }
        }

        // 用户查看指定物品的评论 参数为item_id 
        //需要item_comments表和users表连表查询 返回List<Tuple<Users,Item_Comments>>

        public List<Tuple<Users, Item_Comments>> ViewItemComments(string itemId)
        {
            try
            {
                // 构造查询语句
                
                string fromClause = "ITEM_COMMENTS LEFT OUTER JOIN USERS ON ITEM_COMMENTS.USER_ID = USERS.USER_ID";
                string whereClause = "ITEM_ID = :itemId";
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter("itemId", itemId)
                };

                var result = CommentBusiness.QueryTableWithFromAndWhereBusiness(fromClause, whereClause, parameters);

                // 将查询结果转换为 List<Tuple<Users, Item_Comments>>
                List<Tuple<Users, Item_Comments>> commentsList = new List<Tuple<Users, Item_Comments>>();

                foreach (var row in result)
                {
                    Users user = UserBusiness.MapDictionaryToObject(row);
                    if (user.User_Name == "{}")
                    {
                        user.User_Name = "";
                    }
                    Item_Comments comment = CommentBusiness.MapDictionaryToObject(row);
                    commentsList.Add(new Tuple<Users, Item_Comments>(user, comment));
                }

                return commentsList;
            }
            catch (Exception ex)
            {
                throw new Exception($"用户查看评论时发生错误：{ex.Message}");
            }
        }
    }


}


