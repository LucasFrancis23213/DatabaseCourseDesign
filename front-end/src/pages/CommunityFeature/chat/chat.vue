<script lang="ts" setup>
import {onMounted, onUnmounted, onUpdated, ref, nextTick} from 'vue';
import ChatBubble from "@/components/CommunityFeature/chat/ChatBubble.vue";
import MessageInput from "@/components/CommunityFeature/chat/MessageInput.vue";
import apiService from './apiService';
import axios from "axios";
import {useRoute} from 'vue-router';
import {useAccountStore} from '@/store/account';

const {account, permissions} = useAccountStore();


axios.defaults.baseURL = import.meta.env.VITE_API_URL;

const route = useRoute();

const conversation_id = ref(+route.params.conversation_id);
const current_user_id = ref(+account.userId);

const other_avatar = ref();
const other_name = ref();

const messages = ref([]);

const isSending = ref(false);

const getMessages = async () => {
  try {
    const response = await apiService.getMessages(conversation_id.value, +current_user_id.value);
    messages.value = response.messages;
    other_name.value = response.name;
    other_avatar.value = response.avatar;
  } catch (error) {
    alert("获取消息列表出现错误");
  }
};
const beijingTime = new Date(new Date().getTime() + 8 * 3600 * 1000);
const formattedTime = beijingTime.toISOString().replace('Z', '+08:00');

async function handleSendMessage(message) {
  if (!message.trim()) {
    return; // 如果消息为空，则不发送
  }
  // 创建消息对象
  const newMessage = {
    id: parseInt(Date.now().toString()), // 使用当前时间戳作为临时ID
    conversation_id: conversation_id.value,
    content: message,
    type: 'text',
    time: formattedTime,
    sender: +current_user_id.value,
    current_user_id: +current_user_id.value, // 假设这是用户发送的消息
  };
  console.log(newMessage.time)
  displayMessage(newMessage);

  try {
    // 发送消息到后台
    const response = await apiService.sendMessage(newMessage);

    // 如果后台返回了更新后的消息（例如，带有服务器生成的ID），则更新前端显示
    if (response) {
      updateDisplayedMessage(newMessage.id, response.message_id);
    }
  } catch (error) {
    alert("消息发送失败，请稍后重试");
  }
}

function displayMessage(message) {
  // 实现显示消息的逻辑
  messages.value.push(message);
}


// 辅助函数：更新已显示的消息的id
function updateDisplayedMessage(oldId, newId) {
  const oldMessage = messages.value.filter(msg => msg.id === oldId).shift();
  oldMessage.id = newId;

}

function isSelf(sender_id) {
  return sender_id === current_user_id.value;
}

async function updateReadStatus() {
  /*更新消息的读取状态*/
  try {
    await apiService.updateReadStatus(conversation_id.value, current_user_id.value);
  } catch (e) {
    alert(`更新已读状态失败，错误信息为：${e}`);
  }
}

async function retractMessage(message_id) {
  /*撤回消息*/
  // 向后台发送消息处理撤回逻辑
  try {
    await apiService.retractMessage(message_id, current_user_id.value);
  } catch (e) {
    alert(`撤回失败，请重试`);
  }
  // 前台移除该消息
  removeMessage(message_id)
}

const connection = ref(null);
const socket = ref(null);
const connectionStatus = ref('Disconnected');
const errorStatus = ref('');

const connect = () => {
  socket.value = new WebSocket(`wss://localhost:44343/ws?user_id=${current_user_id.value}`);

  socket.value.onopen = () => {
    connectionStatus.value = 'Connected';
    errorStatus.value = '';
  };

  socket.value.onclose = () => {
    connectionStatus.value = 'Disconnected';
    setTimeout(connect, 3000);//断连后三秒重连
  };

  socket.value.onerror = (error) => {
    errorStatus.value = `WebSocket Error: ${error.message}`;
  };

  socket.value.onmessage = (event) => {
    receiveMessage(event.data);
  };
};

function convertMessageFormat(originalMessage) {
  //解析接收消息的格式并更改为此组件中使用的message格式
  const msg = JSON.parse(originalMessage);
  console.log(msg);
  return {
    id: msg.Message_ID,
    conversation_id: conversation_id.value,
    content: msg.Message_Content,
    type: msg.Message_Type,
    time: msg.Send_Time,
    sender: msg.Sender_User_ID,
    current_user_id: msg.Receiver_User_ID
  };
}

function removeMessage(message_id: Number) {
  messages.value = messages.value.filter(msg => msg.id !== message_id)
}

const receiveMessage = (message) => {
  //接收消息
  let newMessage = convertMessageFormat(message);
  if (newMessage.type === 'retract') {
    removeMessage(newMessage.id);
  } else if (newMessage.type === 'text') {
    displayMessage(newMessage);
  }
};
const disconnect = () => {
  //断开连接
  if (socket.value) {
    socket.value.close();
    socket.value = null;
  }
};
const checkConnection = () => {
  //检查连接状态的函数
  if (socket.value) {
    switch (socket.value.readyState) {
      case WebSocket.CONNECTING:
        connectionStatus.value = 'Connecting...';
        break;
      case WebSocket.OPEN:
        connectionStatus.value = 'Connected';
        break;
      case WebSocket.CLOSING:
        connectionStatus.value = 'Closing...';
        break;
      case WebSocket.CLOSED:
        connectionStatus.value = 'Disconnected';
        break;
    }
  } else {
    connectionStatus.value = 'No connection';
  }
  console.log(connectionStatus.value);
};

const chatContainerRef = ref(null);
const scrollToBottom = () => {
  nextTick(() => {
    if (chatContainerRef.value) {
      chatContainerRef.value.scrollTop = chatContainerRef.value.scrollHeight
    }
  })
}

onMounted(() => {
  updateReadStatus();
  getMessages();
  scrollToBottom();
  connect();
})

onUpdated(() => {
  scrollToBottom();

})

onUnmounted(() => {
  disconnect();
})


</script>

<template>
  <div class="chat-container" ref="chatContainerRef">
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
          @follow=""
          @retract="retractMessage"
      />
    </div>
    <button @click="checkConnection">检查连接</button>

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

.message-input {

  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
}
</style>