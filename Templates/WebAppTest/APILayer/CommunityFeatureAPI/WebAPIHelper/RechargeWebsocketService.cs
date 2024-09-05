using SQLOperation.PublicAccess.Utilities;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DatabaseProject.APILayer.CommunityFeatureAPI
{
    public class RechargeWebsocketService
    {
        // 用于存储用户ID（整数类型）与WebSocket连接的映射
        private readonly ConcurrentDictionary<int, WebSocket> _connections = new ConcurrentDictionary<int, WebSocket>();
        // 用于存储用户ID与recharge_id的映射
        private readonly Dictionary<string,int> _rechargeIdToUserId = new Dictionary<string,int>();
        // 处理WebSocket连接请求
        public async Task HandleConnection(HttpContext context)
        {
            // 确保请求是WebSocket请求
            if (context.WebSockets.IsWebSocketRequest)
            {
                // 接受WebSocket请求并获取WebSocket实例
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();

               
                // 从查询参数中获取user_id
                if (!int.TryParse(context.Request.Query["user_id"], out int userId) )
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

                // 将接收到的字节数组转换为字符串
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                // 解析 JSON 消息
                using (JsonDocument doc = JsonDocument.Parse(message))
                {
                    var root = doc.RootElement;

                    // 获取类型
                    if (root.TryGetProperty("type", out JsonElement typeElement) &&
                        typeElement.GetString() == "start_monitoring")
                    {
                        // 获取 rechargeId 和 userId
                        if (root.TryGetProperty("rechargeId", out JsonElement rechargeIdElement) &&
                            root.TryGetProperty("userId", out JsonElement userIdElement))
                        {
                            string recharge_id = rechargeIdElement.GetString();
                           
                            int user_id = userIdElement.GetInt32();

                            if (recharge_id != null)
                            {
                                // 插入成字典形式
                                _rechargeIdToUserId[recharge_id] = user_id;
                            }
                            else
                            {
                                throw new Exception("recharge_id不合法");
                            }
                           
                        }
                    }
                }

            } while (!result.CloseStatus.HasValue);

            // 移除关闭的连接
            _connections.TryRemove(userId, out _);
        }

        // 向指定用户发送消息
        public async Task SendMessageAsync(string rechargeId, string message)
        {
            // 查找与 rechargeId 相关联的 userId
            if (_rechargeIdToUserId.TryGetValue(rechargeId, out int userId))
            {
                // 查找 userId 对应的 WebSocket 连接
                if (_connections.TryGetValue(userId, out var webSocket))
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    var segment = new ArraySegment<byte>(buffer);

                    // 发送消息
                    await webSocket.SendAsync(segment, WebSocketMessageType.Text, endOfMessage: true, cancellationToken: CancellationToken.None);
                }
                else
                {
                    // WebSocket 连接不存在的处理逻辑
                    Console.WriteLine($"No WebSocket connection found for user ID {userId}");
                }
            }
            else
            {
                // rechargeId 不存在的处理逻辑
                Console.WriteLine($"No user ID found for recharge ID {rechargeId}");
            }
        }





    }
}
