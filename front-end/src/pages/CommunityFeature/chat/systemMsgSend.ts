import apiService from "@/pages/CommunityFeature/chat/apiService";
const system_user_id = import.meta.env.VITE_SYSTEM_USER_ID


async function sendSystemMsg(targetId:number,content:string,build_chat_user_id:number,sender_sys_user_id=system_user_id){
    const beijingTime = new Date(new Date().getTime() + 8 * 3600 * 1000);
    const formattedTime = beijingTime.toISOString().replace('Z', '+08:00');
    const sysMsg={
        id: +parseInt(Date.now().toString()), // 使用当前时间戳作为临时ID
        conversation_id: +targetId,
        content: JSON.stringify({content:content,build_chat_user_id:build_chat_user_id}),
        type: 'text',
        time: formattedTime,
        sender: +sender_sys_user_id,
        current_user_id: +sender_sys_user_id, // 假设这是用户发送的消息
    }
    try {
        console.log(sysMsg)
    // 发送消息到后台
    const response = await apiService.sendMessage(sysMsg);
  } catch (error) {
    alert("系统消息发送失败，请稍后重试");
  }
}

export default sendSystemMsg