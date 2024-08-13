using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SQLOperation.PublicAccess.Utilities;

namespace DatabaseProject.APILayer.CommunityFeatureAPI
{
    public class WebSocketService
    {
        // 用于存储用户ID（整数类型）与WebSocket连接的映射
        private readonly ConcurrentDictionary<int, WebSocket> _connections = new ConcurrentDictionary<int, WebSocket>();

        // 处理WebSocket连接请求
        public async Task HandleConnection(HttpContext context)
        {
            // 确保请求是WebSocket请求
            if (context.WebSockets.IsWebSocketRequest)
            {
                // 接受WebSocket请求并获取WebSocket实例
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();

                // 从查询参数中获取用户ID
                if (!int.TryParse(context.Request.Query["user_id"], out int userId))
                {
                    context.Response.StatusCode = 400;
                    return;
                }

                // 将用户ID和WebSocket连接存储到字典中
                _connections[userId] = webSocket;

                // 启动异步方法来接收消息
                await ReceiveMessages(userId, webSocket);
            }
            else
            {
                // 如果请求不是WebSocket请求，则返回400状态码
                context.Response.StatusCode = 400;
            }
        }

        // 异步接收并处理消息
        private async Task ReceiveMessages(int userId, WebSocket webSocket)
        {
            // 创建缓冲区以存储接收到的消息
            var buffer = new byte[1024 * 4];

            // 接收初始消息
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                // 将接收到的字节数组转换为字符串
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                // 解析消息内容（假设消息为 JSON 格式）
                User_Messages? userMessages;
                try
                {
                    userMessages = JsonSerializer.Deserialize<User_Messages>(message);
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"消息解析失败: {ex.Message}");
                    continue; // 跳过处理此消息
                }

                // 处理接收到的消息
                // 这里可以对消息内容进行处理，比如将其显示在用户界面上
                //Console.WriteLine($"接收到来自用户 {userId} 的消息: {userMessages.Message_Content}");

                // 接收下一条消息
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            // 移除关闭的WebSocket连接
            _connections.TryRemove(userId, out _);

            // 关闭WebSocket连接
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        // 发送消息
        public async Task SendMessageAsync(int targetUserId, User_Messages user_Messages)
        {
            if (_connections.TryGetValue(targetUserId, out var webSocket))
            {
                if (webSocket.State == WebSocketState.Open)
                {
                    // 序列化 USER_MESSAGES 对象为 JSON 字符串
                    var jsonMessage = JsonSerializer.Serialize(user_Messages);

                    // 将 JSON 字符串转换为字节数组
                    var messageBuffer = Encoding.UTF8.GetBytes(jsonMessage);

                    // 发送消息
                    await webSocket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
