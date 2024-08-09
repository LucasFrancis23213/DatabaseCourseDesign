<script lang="ts" setup>
import {onMounted, onUnmounted, ref, watchEffect} from 'vue';
import ChatBubble from "@/components/CommunityFeature/chat/ChatBubble.vue";
import MessageInput from "@/components/CommunityFeature/chat/MessageInput.vue";
import axios from "axios";
import {useRoute} from 'vue-router';
import * as signalR from '@microsoft/signalr';
import { useAccountStore } from '@/store/account';
const {account, permissions} = useAccountStore();


axios.defaults.baseURL= import.meta.env.VITE_API_URL;

const route =useRoute();

const conversation_id  = ref(+route.params.conversation_id);
const current_user_id = ref(account.userId);//ref(+route.query.current_user_id);

const other_avatar = ref();

const other_name = ref();

const messages = ref([]);

const isSending = ref(false);


async function getMessages(){
  /*获取消息列表*/
  console.log("getMessages");
  try{
    const res = await axios.post(`/api/conversations/${conversation_id.value}`,{
      "conversation_id":conversation_id.value,
      "current_user_id":current_user_id.value,
    });
    console.log(res);
    messages.value = res.data.messages;
    other_name.value = res.data.name;
    other_avatar.value = res.data.avatar;


  }catch (e){
    console.log(e);
    alert(`获取聊天历史记录失败，错误信息为：${e}`);
  }
}
async function handleSendMessage(message) {
  if (!message.trim()) {
    return; // 如果消息为空，则不发送
  }
  // 创建消息对象
  const newMessage = {
    id: Date.now().toString(), // 使用当前时间戳作为临时ID
    conversation_id: conversation_id.value,
    content: message,
    type:'text',
    time: new Date().toISOString(),
    sender:current_user_id.value,
    current_user_id: current_user_id.value, // 假设这是用户发送的消息
  };
  console.log(newMessage);
  // 在前端立即显示消息（回显）
  displayMessage(newMessage);

  try {
    // 发送消息到后台
    const response = await sendMessageToBackend(newMessage);

    // 如果后台返回了更新后的消息（例如，带有服务器生成的ID），则更新前端显示
    if (response.data) {
      updateDisplayedMessage(newMessage.id,response.data.message_id);
    }
  } catch (error) {
    console.error('发送消息到后台失败:', error);
    // 在UI中显示错误消息
    displayErrorMessage('消息发送失败，请稍后重试');
  }
}
function displayMessage(message) {
  // 实现显示消息的逻辑
  messages.value.push(message);
}

async function sendMessageToBackend(message) {
  // 实现发送消息到后台的逻辑
  try {
    const res = await axios.post(`/api/conversations/messages`, message);
    console.log(res);
    return res;
  }catch (e) {
    console.log(e);
    alert(`发送消息失败，错误信息为：${e}`);
  }
}

// 辅助函数：更新已显示的消息的id
function updateDisplayedMessage(oldId,newId) {
  const oldMessage = messages.value.filter(msg => msg.id === oldId).shift();
  oldMessage.id = newId;

}

// 辅助函数：显示错误消息
function displayErrorMessage(errorMessage) {
  // 实现显示错误消息的逻辑
  const errorContainer = document.getElementById('error-container');
  errorContainer.textContent = errorMessage;
  errorContainer.style.display = 'block';
  setTimeout(() => {
    errorContainer.style.display = 'none';
  }, 3000);
}

function isSelf(sender_id){
  return sender_id === current_user_id.value;
}
async function updateReadStatus(){
  /*更新消息的读取状态*/
  try{
    const res = await axios.post(`/api/conversations/${conversation_id.value}/update_read_status`,
        {conversation_id:conversation_id.value,
        current_user_id:current_user_id.value,});
    console.log(res);
  }catch(e){
    console.log(e);
    alert(`更新已读状态失败，错误信息为：${e}`);
  }
}

async function retractMessage(message_id){
  /*撤回消息*/
  console.log(message_id);
  // 处理撤回逻辑
  try{
    const res = await axios.post(`/api/messages/${message_id}/retract`,{
      message_id:+message_id,
      current_user_id:+current_user_id.value,
      time:new Date().toISOString(),
    });
    console.log(res);
  }catch (e){
    console.log(e);
    alert(`撤回失败，错误信息为：${e}`);
  }
  console.log('撤回消息:', message_id)
  // 例如,从messages数组中移除该消息
  messages.value = messages.value.filter(msg => msg.id !== message_id)
}

const connection = ref(null);
const debugInfo = ref('');
const handleReceiveMessage = async (userMessages) => {
  debugInfo.value = JSON.stringify(userMessages, null, 2);

  const receiver_in_window = true; // Replace with actual logic

  try {
    if (userMessages.message_type === "retract") {
      retractMessage(userMessages.message_id, userMessages.sender_user_id);
    } else {
      const response = await updateReadStatus(userMessages.message_id, receiver_in_window, userMessages.sender_user_id);
      const senderUserName = response.data.sender_user_name;

      messages.value.push({
        id: userMessages.message_id,
        senderId: userMessages.sender_user_id,
        senderName: senderUserName,
        content: userMessages.message_content
      });
    }
  } catch (error) {
    console.error('Failed to process message:', error);
  }
};
const connectionStatus = ref('Disconnected');
const errorStatus = ref('');
const connect = async () => {


  connectionStatus.value = "Connecting...";
  errorStatus.value = "";

  connection.value = new signalR.HubConnectionBuilder()
    .withUrl(`${import.meta.env.VITE_API_URL}/chathub`)
    .build();

  connection.value.on("ReceiveMessage", handleReceiveMessage);

  try {
    await connection.value.start();
    console.log("Connected!");
    connectionStatus.value = `Connected as user ${account.userId}`;
    await connection.value.invoke("OnConnectedAsync", parseInt(account.userId));
  } catch (err) {
    connectionStatus.value = "Connection failed.";
    errorStatus.value = `Error: ${err.toString()}`;
    console.error("Connection error:", err.toString());
  }
};
// const startConnection = async () =>{
//   connection.value = new signalR.HubConnectionBuilder()
//       .withUrl(`/chathub`)
//       .build();
//   try{
//     await connection.value.start();
//     console.log("SignalR Connected. ");
//
//     connection.value.on('ReceiveMessage',async (message) => {
//       if (message.data.receiver_user_id === current_user_id) {
//         messages.value.push(message);
//         try {
//           const res = await axios.post(`/api/messages/receive`, {
//             message_id: message.message_id,
//             receiver_in_window: true,
//             sender_user_id: message.sender_user_id,
//           });
//
//           console.log(res);
//         } catch (e) {
//           console.error(e);
//           alert(e);
//         }
//
//       }
//
//     });
//
//
//   } catch (e){
//     console.error("SignalR Connection Error: ");
//     console.error(e);
//   }
//
// }
const disconnect = async () => {
  if (connection.value) {
    try {
      await connection.value.stop();
      console.log("Disconnected!");
      connectionStatus.value = "Disconnected";
    } catch (err) {
      console.error(err.toString());
    }
  }
};

onMounted(()=>{
  updateReadStatus();
  getMessages();
  //startConnection();
  connect();
})

onUnmounted(()=>{
  disconnect();
})




</script>

<template>
  <div class="chat-container">
    <div class="messages-list">
      <ChatBubble
        v-for="msg in messages"
        :key="msg.id"
        :id="msg.id"
        :content="msg.content"
        :time="msg.time"
        :isSelf="isSelf(msg.sender)"
        :avatar="isSelf(msg.sender) ? '' :other_avatar"
        :type="msg.type"
        @retract="retractMessage"
      />
    </div>

    <MessageInput
      :is-sending="isSending"
      @send-message="handleSendMessage"
      class="message-input"
    />
  </div>
</template>

<style scoped>
.chat-container {
  display: flex;
  flex-direction: column;
  padding: 20px;
  box-sizing: border-box;
}

.messages-list {

  flex-grow: 1;
  overflow-y: auto;
  margin-bottom: 20px;
}

.message-input{

  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
}
</style>