using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseProject.BusinessLogicLayer.CommunityFeatureBLL;
using SQLOperation.PublicAccess.Templates.SQLManager;
using SQLOperation.PublicAccess.Utilities;


namespace DatabaseProject.ServiceLayer.ConmmunityFeature
{
    public class UserMessages
    {
        private CommunityFeatureBusiness<User_Messages> UserMessagesBusiness;
        private CommunityFeatureBusiness<User_Conversations> UserConversationsBusiness;
        private CommunityFeatureBusiness<User_Relationships> UserRelationshipsBusiness;

        private List<string> userMessagesList= new List<string> { "message_content","read_status","send_time","message_type","conversation_id"};
        private List<string> userConversationList=new List<string> {"sender_user_id","receiver_user_id","unread_message_count","last_message_id","receiver_in_window" };

        // 构造函数
        public UserMessages(Connection connection)
        {
            UserMessagesBusiness = new CommunityFeatureBusiness<User_Messages>(connection);
            UserConversationsBusiness = new CommunityFeatureBusiness<User_Conversations>(connection);
            UserRelationshipsBusiness = new CommunityFeatureBusiness<User_Relationships>(connection);
        }

        // 获取会话列表 userId 可能为 sender_user_id 或 receiver_user_id
        public List<User_Conversations> GetConversations(int userId)
        {
            try
            {
                var senderConversations = UserConversationsBusiness.QueryBusiness(new Dictionary<string, object> { { "sender_user_id", userId } });
                var receiverConversations = UserConversationsBusiness.QueryBusiness(new Dictionary<string, object> { { "receiver_user_id", userId } });
                var allConversations = senderConversations.Concat(receiverConversations).ToList();
                return allConversations;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving conversations for user ID {userId}: {ex.Message}");
                throw new Exception($"Error retrieving conversations for user ID {userId}: {ex.Message}");
            }
        }

        // 根据会话id获取所有消息
        public List<User_Messages> GetAllMessagesByConversationId(int conversationId)
        {
            try
            {
                return UserMessagesBusiness.QueryBusiness(new Dictionary<string, object> { { "conversation_id", conversationId } });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving messages for conversation ID {conversationId}: {ex.Message}");
                throw new Exception($"Error retrieving messages for conversation ID {conversationId}: {ex.Message}");
            }
        }

        // 根据会话ID获取最后一条消息信息 写成try_catch代码块形式
        public User_Messages GetLastMessageInfoByConversationId(int conversationId)
        {
            try
            {
                // 根据会话ID查询消息
                var messages = UserMessagesBusiness.QueryBusiness(new Dictionary<string, object>
                {
                    { "conversation_id", conversationId }
                });

                // 如果没有找到消息，返回null
                if (messages == null || messages.Count == 0)
                {
                    return null;
                }

                // 假设消息按时间排序，获取最后一条消息
                var lastMessage = messages.OrderByDescending(m => m.Send_Time).FirstOrDefault();

                return lastMessage;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving last message for conversation {conversationId}: {ex.Message}");
                throw new Exception($"Error retrieving last message for conversation {conversationId}: {ex.Message}");
            }
        }

        //根据会话ID获取所有未读消息
        public List<User_Messages> GetUnreadMessagesByConversationId(int conversationId)
        {
            try
            {
                var messages = UserMessagesBusiness.QueryBusiness(new Dictionary<string, object> { { "conversation_id", conversationId },{ "read_status","N"} });
                return messages;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving unread messages for conversation ID {conversationId}: {ex.Message}");
                throw new Exception($"Error retrieving unread messages for conversation ID {conversationId}: {ex.Message}");
            }
        }


        //根据会话对象ID 消息内容 消息类型 发送消息 
        public int SendMessage(int receiverUserId, string messageContent, string messageType,int senderUserId)
        {
            try
            {
                // 首先查询会话ID
                var conversations = UserConversationsBusiness.QueryBusiness(new Dictionary<string, object> { {"receiver_user_id",receiverUserId},{"sender_user_id",senderUserId } });
                var conversation=conversations.FirstOrDefault();
                if (conversation != null) {
                    ;
                }
                else
                {
                    // 如果会话不存在，首先创建一个新的会话
                    conversation = UserConversationsBusiness.PackageData(
                        0, // 假设会话ID是自动生成的，初始化为0
                        senderUserId,
                        receiverUserId,
                        "N"
               
                    );
                    int conversationId = UserConversationsBusiness.AddBusiness(userConversationList, "conversation_id", conversation);
                   
                }

                // 首先创建message
                var message = UserMessagesBusiness.PackageData(0, conversation.Conversation_ID, messageContent, conversation.Receiver_In_Window, DateTime.Now, messageType);
                int message_id = UserMessagesBusiness.AddBusiness(userConversationList, "message_id", message);

                // 返回message_id
                return message_id;


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to receiver user ID {receiverUserId}: {ex.Message}");
                return 0;
            }
        }

       

        // 根据消息ID 先查询消息 判断消息的发送时间是否在5min之内 撤回消息 否则撤回失败
        public bool WithdrawMessage(int messageId,int userId)
        {
            try
            {
                var messages = UserMessagesBusiness.QueryBusiness(new Dictionary<string, object> { { "message_id", messageId },{ "sender_user_id",userId} });
                var message = messages.FirstOrDefault();

                if (message != null && (DateTime.Now - message.Send_Time).TotalMinutes <= 5)
                {
                    return UserMessagesBusiness.RemoveBusiness(new Dictionary<string, object> { { "message_id", messageId } });
                }
                else
                {
                    throw new Exception("Cannot withdraw message. Either message not found or more than 5 minutes have passed.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error withdrawing message with ID {messageId}: {ex.Message}");
                return false;
            }
        }

        // 以下待定
        //根据对方用户id 请求关系类型 操作类型 发送请求关系的消息
        //操作类型为request 则消息类型为relationship_request 操作类型为delete 则消息类型为relationship_delete

        //根据消息ID和回复动作 处理关系 
        // 消息类型为relationship_request 回复动作为accept 即为接受 把这个关系加入关系表中 
        // 其他情况以此类推

        //根据要查询的用户ID和查询的关系类型查询关系列表

    }
}
