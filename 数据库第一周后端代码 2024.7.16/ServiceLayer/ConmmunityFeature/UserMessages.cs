using DatabaseProject.BusinessLogicLayer.CommunityFeatureBLL;
using Microsoft.VisualBasic;
using Oracle.ManagedDataAccess.Client;
using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Utilities;


namespace DatabaseProject.ServiceLayer.ConmmunityFeature
{
    public class UserMessages
    {
        private CommunityFeatureBusiness<User_Messages> UserMessagesBusiness;
        private CommunityFeatureBusiness<User_Conversations> UserConversationsBusiness;
        
        private CommunityFeatureBusiness<Users> UsersBusiness;
        

        private List<string> userMessagesList = new List<string> { "message_content", "read_status", "send_time", "message_type", "sender_user_id","receiver_user_id" };
        private List<string> userConversationList = new List<string> { "user1_id","user2_id","last_message_id","unread_count" };

        // 构造函数
        public UserMessages(Connection connection)
        {
            UserMessagesBusiness = new CommunityFeatureBusiness<User_Messages>(connection);
            UserConversationsBusiness = new CommunityFeatureBusiness<User_Conversations>(connection);
            UsersBusiness=new CommunityFeatureBusiness<Users>(connection);
           
        }

        
        // 查找涉及到特定 user_id 的所有会话并合并 修改
        public List<Conversation_User_Message> FindAllUserConversations(int userId)
        {
            try
            {
                string fromClause = @"
                     USER_CONVERSATIONS 
                     JOIN USER_MESSAGES ON LAST_MESSAGE_ID = MESSAGE_ID
                     JOIN USERS ON (USER1_ID = USER_ID OR USER2_ID = USER_ID)";


                // 定义 WHERE 子句，包括查询条件
                string whereClause = $"(USER1_ID = :userId OR USER2_ID = :userId) AND USER_ID != :userId";


                // 定义参数
                OracleParameter[] parameters = new OracleParameter[]
                {
                     new OracleParameter(":userId", userId)
                };

                // 调用查询方法
                List<Dictionary<string, object>> rowList = UserConversationsBusiness.QueryTableWithFromAndWhereBusiness(fromClause, whereClause, parameters);

                // 使用字典存储已合并的会话，key为另一个用户的ID，value为Conversation_User_Message对象
                Dictionary<int, Conversation_User_Message> mergedConversations = new Dictionary<int, Conversation_User_Message>();

                foreach (var row in rowList)
                {
                    // 将数据映射到 User_Conversations、User_Messages 和 Users 对象
                    User_Conversations conversation = UserConversationsBusiness.MapDictionaryToObject(row);
                    User_Messages message = UserMessagesBusiness.MapDictionaryToObject(row);
                    Users userInfo = UsersBusiness.MapDictionaryToObject(row);

                    // 确定另一个用户的ID
                    int otherUserId = userInfo.User_ID;

                    // 如果已经存在相同的会话，则比较消息的时间，保留最近的消息
                    if (mergedConversations.ContainsKey(otherUserId))
                    {
                        var existingConversation = mergedConversations[otherUserId];
                        if (message.Send_Time > existingConversation.userLastMessage.Send_Time)
                        {
                            if (conversation.User2_ID == userId)
                            {
                                // 更新整个会话和消息
                                existingConversation.userConversations = conversation;
                                existingConversation.userLastMessage = message;
                            }
                            else
                            {
                                // 更新消息，但保持会话不变
                                existingConversation.userLastMessage = message;
                            }
                        }
                    }
                    else
                    {

                        // 否则，直接添加新的会话和消息 接收方不是userId 先放0 
                        if (conversation.User2_ID != userId)
                        {
                            conversation.Unread_Count = 0;
                        }
                        mergedConversations[otherUserId] = new Conversation_User_Message
                        {
                            userConversations = conversation,
                            userLastMessage = message,
                            userInfo = userInfo
                        };


                    }

                   
                }

                
                // 将合并后的会话转换为列表并按最后一条消息时间由近到远排序返回
                return mergedConversations.Values
                                          .OrderByDescending(conversation => conversation.userLastMessage.Send_Time)
                                          .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"查找涉及用户 {userId} 的会话时出错: {ex.Message}");
                throw new Exception($"查找涉及用户 {userId} 的会话时出错: {ex.Message}");
            }
        }

        // 根据用户ID查找指定用户
        public Users GetUserInfo(int userId)
        {
            try
            {
                Dictionary<string, object> condition = new Dictionary<string, object>
                {
                    {"user_id",userId }
                };
                var result = UsersBusiness.QueryBusiness(condition,"AND");

                return result.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception($"获取用户信息出错：{ex.Message}");
            }
        }

        // 根据指定消息ID查找消息
        public User_Messages FindUserMessage(int messageId)
        {
            try
            {
                // 构建查询条件
                Dictionary<string, object> condition = new Dictionary<string, object>
                    {
                        { "message_id", messageId }
                    };

                // 调用查询方法
                List<User_Messages> messages = UserMessagesBusiness.QueryBusiness(condition,"AND");

                // 如果查询到消息，则返回第一条消息（假设消息ID是唯一的）
                return messages.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finding message with id {messageId}: {ex.Message}");
                throw new Exception($"Error finding message with id {messageId}: {ex.Message}");
            }
        }

        // 获取一对用户之间的所有消息
        public List<User_Messages> FindAllMessagesBetweenUsers(int localUserId, int chatUserId)
        {
            try
            {
                // 查询所有与指定用户相关的消息，无论是发送方还是接收方
                string whereClause = $"(SENDER_USER_ID = :userId1 AND RECEIVER_USER_ID = :userId2) OR (SENDER_USER_ID = :userId2 AND RECEIVER_USER_ID = :userId1)";
                OracleParameter[] parameters = new OracleParameter[]
                {
                    new OracleParameter(":userId1", localUserId),
                    new OracleParameter(":userId2", chatUserId)
                };


                // 调用封装的数据库查询方法
                List<User_Messages> messages = UserMessagesBusiness.QueryTableWithWhereBusiness(whereClause, parameters);
                // 在外部对消息列表按时间进行排序，从远到近
                messages.Sort((msg1, msg2) => DateTime.Compare(msg1.Send_Time, msg2.Send_Time));

                return messages ?? new List<User_Messages>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error finding messages between users {localUserId} and {chatUserId}: {ex.Message}");
                throw new Exception($"Error finding messages between users {localUserId} and {chatUserId}: {ex.Message}");
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
                    { "sender_user_id", conversationId },
                    {"read_status","N" }
                };

                // 调用更新方法
                return UserMessagesBusiness.UpdateBusiness(new Dictionary<string, object> { { "read_status", "Y" } },condition);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新会话消息状态为已读时出错: {ex.Message}");
                throw new Exception($"更新会话消息状态为已读时出错: {ex.Message}");
            }
        }

        // 根据指定messageId更新message的阅读状态
        public bool UpdateMessageReadStatus(int messageId)
        {
            try
            {
                // 构造更新条件
                Dictionary<string, object> condition = new Dictionary<string, object>
                {
                    { "message_id", messageId }
                };

                // 调用更新方法，将消息的阅读状态更新为已读
                return UserMessagesBusiness.UpdateBusiness(new Dictionary<string, object> { { "read_status", "Y" } },condition);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"更新消息ID为 {messageId} 的阅读状态时出错: {ex.Message}");
                throw new Exception($"更新消息ID为 {messageId} 的阅读状态时出错: {ex.Message}");
            }
        }


        //输入当前用户ID 接收用户ID 消息内容和消息类型 发送消息
        public User_Messages SendMessage(int senderUserId, int receiverUserId, string messageContent, string messageType,DateTime time)
        {
            try
            {
                User_Messages message = UserMessagesBusiness.PackageData(0,messageContent,"N",time,messageType,senderUserId,receiverUserId);

                int messageId = UserMessagesBusiness.AddBusiness(userMessagesList, "message_id", message);

                message.Message_ID= messageId;

                return message;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"发送消息时出错: {ex.Message}");
                throw new Exception($"发送消息时出错: {ex.Message}");
            }
        }

        //根据用户ID和对应的消息ID撤回消息    待优化
        public User_Messages WithdrawMessage(int messageId, int userId,DateTime time)
        {
            try
            {
                

                Dictionary<string, object> condition = new Dictionary<string, object>
                {
                    { "message_id", messageId },
                    { "sender_user_id", userId }
                };

                var result = UserMessagesBusiness.QueryBusiness(condition, "AND");
                var message= result.FirstOrDefault();

                if (message == null)
                {
                    throw new Exception("指定消息不存在");
                }

                if ((time - message.Send_Time).TotalMinutes > 5)
                {
                    throw new Exception("消息已超过5分钟无法撤回。");
                }

                Dictionary<string, object> updateColumns = new Dictionary<string, object>
                {
                    {"message_content","撤回了一条信息" },
                    {"message_type","retract" },
                    {"read_status","Y" }
                };

                // 如果成功撤回
                if (UserMessagesBusiness.UpdateBusiness(updateColumns, condition))
                {
                    message.Message_Content = "撤回了一条消息";
                    message.Message_Type = "retract";
                    message.Read_Status = "Y";
                    
                    
                    return message;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"撤回消息时出错: {ex.Message}");
                throw new Exception($"撤回消息时出错: {ex.Message}");
            }
        }


    }
}
