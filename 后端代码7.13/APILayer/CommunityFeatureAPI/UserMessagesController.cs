using DatabaseProject.APILayer.CommunityFeatureAPI;
using DatabaseProject.ServiceLayer.ConmmunityFeature;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SQLOperation.PublicAccess.Utilities;

namespace WebApplication1.APILayer.CommunityFeatureAPI
{
    [Route("api/")]
    [ApiController]
    public class UserMessagesController : ControllerBase
    {
        private UserMessages userMessages { get; set; }
        private readonly IHubContext<ChatHub> _hubContext;
        public UserMessagesController(UserMessages user_messages, IHubContext<ChatHub> hubContext)
        {
            userMessages = user_messages;
            _hubContext = hubContext;
        }

        
        // 获取所有会话
        [HttpPost]
        [Route("conversations")]
        public IActionResult GetConversations([FromBody] int current_user_id)
        {
            try
            {
                // 获取当前用户的所有消息
                List<User_Messages> allMessages = userMessages.FindAllUserMessages(current_user_id);

                // 获取所有包含未读消息的会话
                List<Tuple<int, User_Messages>> conversations = userMessages.FindConversationsWithLastMessage(current_user_id, allMessages);

                // 构建返回结果
                List<object> result = new List<object>();

                foreach (var conversation in conversations)
                {
                    int otherUserId = conversation.Item1;
                    User_Messages lastMessage = conversation.Item2;

                    // 获取对方用户信息（假设从数据库或其他服务中获取）
                    string name = ""; // 获取对方用户姓名
                    string avatar = ""; // 获取对方用户头像

                    // 构建会话对象
                    var conversationObject = new
                    {
                        id = otherUserId,
                        name = name,
                        avatar = avatar,
                        last_message = lastMessage.Message_Content,
                        last_message_time = lastMessage.Send_Time,
                        unread_count = userMessages.GetUnreadMessagesCount(otherUserId,current_user_id, allMessages)
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
        public IActionResult GetConversationMessages(int conversation_id, [FromBody] Dictionary<string, object> request)
        {
            try
            {
                if (request == null || !request.ContainsKey("current_user_id"))
                {
                    return BadRequest(new { status = "error", message = "Invalid request body or missing current_user_id." });
                }

                int current_user_id = Convert.ToInt32(request["current_user_id"]);

                // 查询会话内容
                var conversationMessages = userMessages.FindAllMessagesBetweenUsers(conversation_id, current_user_id);

                if (conversationMessages == null || conversationMessages.Count == 0)
                {
                    return NotFound(new { status = "error", message = $"Conversation with id {conversation_id} not found or no messages available." });
                }

                // 构建响应数据
                var response = new
                {
                    status = "success",
                    conversation_id = conversation_id,
                    messages = conversationMessages.Select(m => new
                    {
                        id = m.Message_ID.ToString(),
                        sender = m.Sender_User_ID == m.Sender_User_ID,
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
        [Route("messages")]
        public async Task<IActionResult> SendMessage([FromBody] Dictionary<string, object> request)
        {
            try
            {
                if (request == null || !request.ContainsKey("sender_user_id") || !request.ContainsKey("receiver_user_id")
                    || !request.ContainsKey("message_content") || !request.ContainsKey("message_type"))
                {
                    return BadRequest(new { status = "error", message = "Invalid request body." });
                }

                // 解析请求参数
                int senderUserId = Convert.ToInt32(request["sender_user_id"]);
                int receiverUserId = Convert.ToInt32(request["receiver_user_id"]);
                string messageContent = Convert.ToString(request["message_content"]);
                string messageType = Convert.ToString(request["message_type"]);

                // 发送消息
                bool success = userMessages.SendMessage(senderUserId, receiverUserId, messageContent, messageType);

                if (success)
                {
                    // 向所有客户端发送消息
                    await _hubContext.Clients.All.SendAsync("ReceiveMessage", senderUserId, messageContent, messageType);
                    return Ok(new { status = "success", message = "Message sent successfully." });
                }
                else
                {
                    return BadRequest(new { status = "error", message = "Failed to send message." });
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
        public IActionResult RetractMessage(string message_id, [FromBody] Dictionary<string, object> request)
        {
            try
            {
                if (request == null || !request.ContainsKey("current_user_id"))
                {
                    return BadRequest(new { status = "error", message = "Invalid request body or missing current_user_id." });
                }

                int current_user_id = Convert.ToInt32(request["current_user_id"]);

                // 尝试撤回消息
                bool success = userMessages.WithdrawMessage(Convert.ToInt32(message_id), current_user_id);

                if (success)
                {
                    // 撤回成功，返回成功状态和撤回时间
                    var response = new
                    {
                        status = "success",
                        time = DateTime.UtcNow // 撤回时间，使用 UTC 时间
                    };
                    return Ok(response);
                }
                else
                {
                    // 撤回失败，返回错误状态和消息
                    return BadRequest(new { status = "error", message = $"Failed to retract message with id {message_id}." });
                }
            }
            catch (Exception ex)
            {
                // 处理异常情况，返回服务器错误状态和异常信息
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "error", message = ex.Message });
            }
        }

        // 退出或者进入会话
        // API层方法：退出和进入会话时更新消息状态为已读
        [HttpPost]
        [Route("conversations/{conversation_id}/update_read_status")]
        public IActionResult UpdateConversationMessagesReadStatus(int conversation_id, [FromBody] Dictionary<string, object> request)
        {
            try
            {
                if (request == null || !request.ContainsKey("current_user_id"))
                {
                    return BadRequest(new { status = "error", message = "Invalid request body or missing current_user_id." });
                }

                int current_user_id = Convert.ToInt32(request["current_user_id"]);

                // 调用SL层方法更新会话中所有消息的未读状态为已读
                bool success = userMessages.UpdateMessagesReadStatus(current_user_id, conversation_id);

                if (success)
                {
                    return Ok(new { status = "success", message = "Update read status successfully." });
                }
                else
                {
                    return BadRequest(new { status = "error", message = "Failed to update read status." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { status = "error", message = ex.Message });
            }
        }





    }
}