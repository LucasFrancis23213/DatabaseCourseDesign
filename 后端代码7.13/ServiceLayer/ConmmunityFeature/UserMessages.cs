using DatabaseProject.BusinessLogicLayer.CommunityFeatureBLL;
using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Utilities;


namespace DatabaseProject.ServiceLayer.ConmmunityFeature
{
    public class UserMessages
    {
        private CommunityFeatureBusiness<User_Messages> UserMessagesBusiness;
        
        private CommunityFeatureBusiness<User_Relationships> UserRelationshipsBusiness;

        private List<string> userMessagesList = new List<string> { "message_content", "read_status", "send_time", "message_type", "conversation_id" };
        private List<string> userConversationList = new List<string> { "sender_user_id", "receiver_user_id", "unread_message_count", "last_message_id", "receiver_in_window" };

        // 构造函数
        public UserMessages(Connection connection)
        {
            UserMessagesBusiness = new CommunityFeatureBusiness<User_Messages>(connection);
            
           
        }

        //查找所有涉及到用户userId的消息
        public List<User_Messages> FindAllUserMessages(int userId)
        {
            try
            {
                Dictionary<string, object> condition = new Dictionary<string, object>
                {
                    { "sender_user_id", userId },
                    { "receiver_user_id", userId }
                };

                return UserMessagesBusiness.QueryBusiness(condition,"OR");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"查找涉及用户 {userId} 的消息时出错: {ex.Message}");
                throw new Exception($"查找涉及用户 {userId} 的消息时出错: {ex.Message}");
            }
        }

        // 返回指定会话的所有未读消息数量
        public int GetUnreadMessagesCount(int otherId, int userId, List<User_Messages> allUserMessages)
        {
            try
            {
                // 计算未读消息的数量
                int unreadMessagesCount = allUserMessages
                    .Count(m => (m.Sender_User_ID == otherId && m.Receiver_User_ID == userId && m.Read_Status == "N") );

                return unreadMessagesCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving unread messages count for user {userId} and other user {otherId}: {ex.Message}");
                throw new Exception($"Error retrieving unread messages count for user {userId} and other user {otherId}: {ex.Message}");
            }
        }

        // 查找会话 返回list
        public List<Tuple<int, User_Messages>> FindConversationsWithLastMessage(int userId,List<User_Messages> messages)
        {
            try
            {
               

                // 构建结果列表
                List<Tuple<int, User_Messages>> result = new List<Tuple<int, User_Messages>>();

                // 找到与当前用户交互的所有用户ID
                var distinctUserIds = messages
                    .Where(m => m.Sender_User_ID == userId || m.Receiver_User_ID == userId)
                    .Select(m => m.Sender_User_ID == userId ? m.Receiver_User_ID : m.Sender_User_ID)
                    .Distinct()
                    .ToList();

                // 对每个用户ID找到最近一条消息
                foreach (var otherUserId in distinctUserIds)
                {
                    // 找到与当前用户交互的最后一条消息
                    var lastMessage = messages
                        .Where(m => (m.Sender_User_ID == userId && m.Receiver_User_ID == otherUserId) ||
                                    (m.Sender_User_ID == otherUserId && m.Receiver_User_ID == userId))
                        .OrderByDescending(m => m.Send_Time) // 按发送时间倒序排序
                        .FirstOrDefault();

                    if (lastMessage != null)
                    {
                        // 添加到结果列表中
                        result.Add(new Tuple<int, User_Messages>(otherUserId, lastMessage));
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"查找带有最后一条消息的对话时出错: {ex.Message}");
                throw new Exception($"查找带有最后一条消息的对话时出错: {ex.Message}");
            }
        }

        // 根据一对用户查找所有消息
        public List<User_Messages> FindAllMessagesBetweenUsers(int userId1, int userId2)
        {
            try
            {
                // 查询所有与指定用户相关的消息，无论是发送方还是接收方
                string whereClause = $"(SENDER_USER_ID = :userId1 AND RECEIVER_USER_ID = :userId2) OR (SENDER_USER_ID = :userId2 AND RECEIVER_USER_ID = :userId1)";
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter(":userId1", userId1),
                    new OracleParameter(":userId2", userId2)
                };

                // 调用封装的数据库查询方法
                List<User_Messages> messages = UserMessagesBusiness.QueryTableWithWhereBusiness("USER_MESSAGES", whereClause, parameters);

                return messages ?? new List<User_Messages>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finding messages between users {userId1} and {userId2}: {ex.Message}");
                throw new Exception($"Error finding messages between users {userId1} and {userId2}: {ex.Message}");
            }
        }

        // 更新会话中未读消息的状态
        public bool UpdateMessagesReadStatus(int currentUserId, int conversationId)
        {
            try
            {
                // 构造更新条件
                Dictionary<string, object> condition = new Dictionary<string, object>
                {
                    { "receiver_user_id", currentUserId },
                    { "conversation_id", conversationId },
                    { "read_status", "N" } // 更新未读消息状态为已读
                };

                // 调用更新方法
                return UserMessagesBusiness.UpdateBusiness(condition, new Dictionary<string, object> { { "read_status", "Y" } });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新会话消息状态为已读时出错: {ex.Message}");
                throw new Exception($"更新会话消息状态为已读时出错: {ex.Message}");
            }
        }


        //输入当前用户ID 接收用户ID 消息内容和消息类型 发送消息
        public bool SendMessage(int senderUserId, int receiverUserId, string messageContent, string messageType)
        {
            try
            {
                User_Messages message = UserMessagesBusiness.PackageData(0,messageContent,"N",DateTime.Now,messageType,senderUserId,receiverUserId);

                int messageId = UserMessagesBusiness.AddBusiness(userMessagesList, "message_id", message);

                return messageId > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送消息时出错: {ex.Message}");
                throw new Exception($"发送消息时出错: {ex.Message}");
            }
        }

        //根据用户ID和对应的消息ID撤回消息
        public bool WithdrawMessage(int messageId, int userId)
        {
            try
            {
                Dictionary<string, object> condition = new Dictionary<string, object>
                {
                    { "message_id", messageId },
                    { "sender_user_id", userId }
                };

                return UserMessagesBusiness.RemoveBusiness(condition);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"撤回消息时出错: {ex.Message}");
                throw new Exception($"撤回消息时出错: {ex.Message}");
            }
        }


    }
}
