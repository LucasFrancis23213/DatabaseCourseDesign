// ChatHub.cs
using DatabaseProject.ServiceLayer.ConmmunityFeature;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using SQLOperation.PublicAccess.Utilities;

namespace DatabaseProject.APILayer.CommunityFeatureAPI
{
    // 创建一个 SignalR Hub 来处理客户端和服务器之间的实时通讯
    public class ChatHub:Hub
    {
        //Clients.All： 表示向所有连接的客户端发送消息。在这个示例中，消息会被广播给所有连接到 ChatHub 的客户端。
        //SendAsync 方法： 这是 SignalR Hub 类的方法，用于发送异步消息给客户端。
        //一个参数 "ReceiveMessage" 是客户端定义的方法名称，用于接收从服务器发送的消息。后续参数 user 和 message 是要发送给客户端的实际数据。
        // 发送消息，并在发送时进行数据库处理

        public async Task SendMessage(User_Messages user_messages)
        {
            await Clients.All.SendAsync("ReceiveMessage", user_messages);
        }
    }
}
