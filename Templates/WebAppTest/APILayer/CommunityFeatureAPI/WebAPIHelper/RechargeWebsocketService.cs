using SQLOperation.PublicAccess.Utilities;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DatabaseProject.APILayer.CommunityFeatureAPI
{
    public class RechargeWebsocketService
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

        // 接收并处理消息
        private async Task ReceiveMessages(int userId, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;

            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            } while (!result.CloseStatus.HasValue);

            // 移除关闭的连接
            _connections.TryRemove(userId, out _);
        }

        // 向指定用户发送消息
        public async Task SendMessageAsync(int userId, string message)
        {
            if (_connections.TryGetValue(userId, out var webSocket))
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                var segment = new ArraySegment<byte>(buffer);

                // 发送消息
                await webSocket.SendAsync(segment, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: CancellationToken.None);
            }
        }




    }
}
