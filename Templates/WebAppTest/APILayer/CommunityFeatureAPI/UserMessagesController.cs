using DatabaseProject.APILayer.CommunityFeatureAPI;
using DatabaseProject.ServiceLayer.ConmmunityFeature;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SQLOperation.PublicAccess.Utilities;
using SQLOperation.PublicAccess.Templates.SQLManager;
using System.Text.Json;

namespace WebApplication1.APILayer.CommunityFeatureAPI
{
    [Route("api/")]
    [ApiController]
    public class UserMessagesController : ControllerBase
    {
        private UserMessages userMessages { get; set; }
        private readonly Connection connection;
        private readonly IHubContext<ChatHub> _hubContext;
        public UserMessagesController(Connection connection, IHubContext<ChatHub> hubContext)
        {
            userMessages = new UserMessages(connection);
            _hubContext = hubContext;
        }

        // 获取所有会话
        [HttpPost]
        [Route("conversations")]
        public IActionResult GetConversations([FromBody] Dictionary<string, JsonElement> requestData)
        {
            try
            {
                int currentUserId = requestData["current_user_id"].GetInt32();
                
                // 获取当前用户的所有会话
                var conversations = userMessages.FindAllUserConversations(currentUserId);


                // 构建返回结果
                List<object> result = new List<object>();

                foreach (var conversation in conversations)
                {
                    var userConversation = conversation.Conversation;
                    var userInfo = conversation.Users;
                   
                    // 根据会话中的 user1_id 和 user2_id 确定对方用户的 ID
                    int otherUserId = currentUserId == userConversation.Sender_User_ID? userConversation.Receiver_User_ID : userConversation.Sender_User_ID;

                    // 构建会话对象
                    var conversationObject = new
                    {
                        id = otherUserId,
                        name = userInfo != null ? userInfo.User_Name : "",
                        avatar = "",
                        last_message = userConversation.Message_Content!= null ? userConversation.Message_Content : "", // 如果有最后一条消息则显示消息内容，否则为空字符串
                        last_message_time = userConversation.Last_Message_Time , // 如果有最后一条消息则显示发送时间，否则为 null
                        unread_count = userConversation.Unread_Count, // 获取未读消息数
                    };

                    result.Add(conversationObject);
                }

                return Ok(new { status = "success", conversations = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "error", message = ex.Message });
            }
        }



        // 获取会话的详细信息
        [HttpPost]
        [Route("conversations/{conversation_id}")]
        public IActionResult GetConversationMessages(int conversation_id, [FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                if (request == null || !request.ContainsKey("current_user_id"))
                {
                    return BadRequest(new { status = "error", message = "请求体格式无效或缺少 current_user_id。" });

                }

                int current_user_id = request["current_user_id"].GetInt32();

                // 查询会话内容
                var conversationMessages = userMessages.FindAllMessagesBetweenUsers(conversation_id, current_user_id);

                var userInfo = userMessages.GetUserInfo(conversation_id);

                // 构建响应数据
                var response = new
                {
                    status = "success",
                    conversation_id = conversation_id,
                    name = userInfo.User_Name,
                    avatar="",
                    messages = conversationMessages.Select(m => new
                    {
                        id = m.Message_ID,
                        sender = m.Sender_User_ID,
                        content = m.Message_Content,
                        time = m.Send_Time,
                        type = m.Message_Type
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "error", message = ex.Message });
            }
        }

        // 发送信息
        [HttpPost]
        [Route("conversations/messages")]
        public async Task<IActionResult> SendMessage([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                if (request == null || !request.ContainsKey("conversation_id") || !request.ContainsKey("current_user_id")
                    || !request.ContainsKey("content") || !request.ContainsKey("type")||!request.ContainsKey("time"))
                {
                    return BadRequest(new { status = "error", message = "无效的请求体。" });

                }

                // 解析请求参数
                
                string messageContent = request["content"].GetString();
                string messageType = request["type"].GetString();

                if (string.IsNullOrEmpty(messageContent) || string.IsNullOrEmpty(messageType))
                {
                    return BadRequest("请求参数无效。'content' 和 'type' 不能为 null 或空字符串。");
                }
                int senderUserId = request["current_user_id"].GetInt32();
                int receiverUserId = request["conversation_id"].GetInt32();
                DateTime time = request["time"].GetDateTime();


                // 发送消息
                User_Messages success = userMessages.SendMessage(senderUserId, receiverUserId, messageContent, messageType,time);

                if (success!=null)
                {
                    // 向userId对应的指定客户端发送消息
                    if (ChatHub.onlineUsers.TryGetValue(receiverUserId, out string connectionId))
                    {
                        // 使用 HubContext 发送消息
                        await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", success);
                       
                    }

                    return Ok(new { status = "success", message_id = success.Message_ID });
                }
                else
                {
                    return BadRequest(new { status = "error", message = "消息发送失败。" });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "error", message = ex.Message });
            }
        }

        // 撤回信息
        [HttpPost]
        [Route("messages/{message_id}/retract")]
        public async Task<IActionResult> RetractMessage(int message_id, [FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                if (request == null || !request.ContainsKey("current_user_id")||!request.ContainsKey("time"))
                {
                    return BadRequest(new { status = "error", message = "无效的请求体" });

                }
                
                int current_user_id = request["current_user_id"].GetInt32();
                DateTime time = request["time"].GetDateTime();


                // 尝试撤回消息
                var success = userMessages.WithdrawMessage(Convert.ToInt32(message_id), current_user_id,time);

                if (success!=null)
                {
                    // 撤回成功，返回成功状态和撤回时间
                    var response = new
                    {
                        status = "success",
                        
                    };
                    // 向指定
                    
                    if (ChatHub.onlineUsers.TryGetValue(success.Receiver_User_ID, out string connectionId))
                    {
                        // 使用 HubContext 发送消息
                        await _hubContext.Clients.Client(connectionId).SendAsync("RetractMessage", success);

                    }

                    return Ok(response);
                }
                else
                {
                    return BadRequest(new { status = "error", message = $"撤回消息失败，消息ID为 {message_id}。" });

                }
            }
            catch (Exception ex)
            {
                // 处理异常情况，返回服务器错误状态和异常信息
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "error", message = ex.Message });
            }
        }

        // 进入会话
        // API层方法：退出和进入会话时更新消息状态为已读
        [HttpPost]
        [Route("conversations/{conversation_id}/update_read_status")]
        public IActionResult UpdateConversationMessagesReadStatus(int conversation_id, [FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                if (request == null || !request.ContainsKey("current_user_id"))
                {
                    return BadRequest(new { status = "error", message = "无效的请求体或缺少 current_user_id。" });

                }

                int current_user_id = request["current_user_id"].GetInt32();

                // 调用SL层方法更新会话中所有消息的未读状态为已读
                bool success = userMessages.UpdateMessagesReadStatus(current_user_id, conversation_id);

                if (success)
                {
                    return Ok();

                }
                else
                {
                    return BadRequest(new { status = "error", message = "更新阅读状态失败。" });

                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "error", message = ex.Message });
            }
        }


        // 接收消息 更新阅读状态为已读
        [HttpPost]
        [Route("messages/receive")]
        public IActionResult ReceiveMessage([FromBody] Dictionary<string, JsonElement> request)
        {
            try
            {
                if (request == null || !request.ContainsKey("message_id") || !request.ContainsKey("receiver_in_window")||!request.ContainsKey("sender_user_id"))
                {
                    return BadRequest(new { status = "error", message = "无效的请求体或缺少message_id或receiver_in_window。" });

                }

                int messageId = request["message_id"].GetInt32();
                bool receiverInWindow = request["receiver_in_window"].GetBoolean();
                int sender_user_id = request["sender_user_id"].GetInt32();

                bool success = false;
                if (receiverInWindow) {
                    // 更新消息的阅读状态为已读
                    success = userMessages.UpdateMessageReadStatus(messageId);
                }
               

                if (!success)
                {
                    return BadRequest(new { status = "error", message = $"更新消息ID为 {messageId} 的阅读状态失败。" });

                }

                var userInfo = userMessages.GetUserInfo(sender_user_id);

                // 构建响应体
                var response = new
                {
                    sender_user_name = userInfo.User_Name,
                    sender_user_avatar = ""
                };


                // 返回成功状态，不返回任何内容
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "error", message = ex.Message });
            }
        }


    }
}