// ChatHub.cs
using DatabaseProject.ServiceLayer.ConmmunityFeature;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using SQLOperation.PublicAccess.Utilities;
using System.Collections.Concurrent;

namespace DatabaseProject.APILayer.CommunityFeatureAPI
{
    // 创建一个 SignalR Hub 来处理客户端和服务器之间的实时通讯
    public class ChatHub:Hub
    {
        //Clients.All： 表示向所有连接的客户端发送消息。在这个示例中，消息会被广播给所有连接到 ChatHub 的客户端。
        //SendAsync 方法： 这是 SignalR Hub 类的方法，用于发送异步消息给客户端。
        //一个参数 "ReceiveMessage" 是客户端定义的方法名称，用于接收从服务器发送的消息。后续参数 user 和 message 是要发送给客户端的实际数据。
        // 发送消息，并在发送时进行数据库处理

        // 在线用户字典，用于存储用户的连接ID
        public static ConcurrentDictionary<int, string> onlineUsers = new ConcurrentDictionary<int, string>();

        public async Task SendMessage(User_Messages user_messages)
        {
            await Clients.All.SendAsync("ReceiveMessage", user_messages);
        }

        // 给特定用户发送消息
        public async Task SendMessageToUser(int userId, User_Messages message)
        {
            if (onlineUsers.TryGetValue(userId, out string connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
            }
        }

        // 覆盖 OnConnectedAsync 方法，在用户连接时调用
        public async Task OnConnectedAsync(int userId)
        {

            string connectionId = Context.ConnectionId;
            onlineUsers[userId] = connectionId;

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // 获取当前连接的连接ID
            string connectionId = Context.ConnectionId;

            // 查找与当前连接ID匹配的用户ID
            int? userId = onlineUsers.FirstOrDefault(x => x.Value == connectionId).Key;

            // 如果找到了用户ID，且连接ID匹配，则从字典中移除该用户的连接信息
            if (userId.HasValue)
            {
                onlineUsers.TryRemove(userId.Value, out _);
            }

            // 调用基类的 OnDisconnectedAsync 方法
            await base.OnDisconnectedAsync(exception);
        }


    }
}
