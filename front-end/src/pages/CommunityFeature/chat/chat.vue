<template>
  <a-layout class="chat-container">
    <a-layout-content class="messages-list" ref="chatContainerRef">
      <ChatBubble
        v-for="msg in messages"
        :key="msg.id"
        :message_id="msg.id"
        :content="msg.content"
        :time="msg.time"
        :isSelf="isSelf(msg.sender)"
        :message_sender_id="msg.sender"
        :avatar="isSelf(msg.sender) ? '' : other_avatar"
        :type="msg.type"
        :user_name="other_name"
        v-model:is_following="is_following"
        @follow="checkFollowingStatus"
        @retract="retractMessage"
      />
    </a-layout-content>
    <a-layout-footer class="message-input-container">
      <a-button @click="checkConnection" class="connection-check-btn">
        检查连接
      </a-button>
      <MessageInput
        :is-sending="isSending"
        @send-message="handleSendMessage"
        class="message-input"
      />
    </a-layout-footer>
  </a-layout>
</template>

<script lang="ts" setup>
import { onMounted, onUnmounted, onUpdated, ref, nextTick } from 'vue';
import { useRoute } from 'vue-router';
import { message } from 'ant-design-vue';
import axios from 'axios';
import ChatBubble from '@/components/CommunityFeature/chat/ChatBubble.vue';
import MessageInput from '@/components/CommunityFeature/chat/MessageInput.vue';
import apiService from './apiService';
import { useAccountStore } from '@/store/account';

const { account } = useAccountStore();
const route = useRoute();

axios.defaults.baseURL = import.meta.env.VITE_API_URL;

const conversation_id = ref(+route.params.conversation_id);
const current_user_id = ref(+account.userId);
const other_avatar = ref('');
const other_name = ref('');
const messages = ref([]);
const isSending = ref(false);
const is_following = ref(false);

const chatContainerRef = ref(null);
const socket = ref(null);
const connectionStatus = ref('Disconnected');

const getMessages = async () => {
  try {
    const response = await apiService.getMessages(conversation_id.value, current_user_id.value);
    messages.value = response.messages;
    other_name.value = response.name;
    other_avatar.value = response.avatar;
  } catch (error) {
    message.error('获取消息列表出现错误');
  }
};

const handleSendMessage = async (messageContent: string) => {
  if (!messageContent.trim()) return;

  const beijingTime = new Date(Date.now() + 8 * 3600 * 1000);
  const formattedTime = beijingTime.toISOString().replace('Z', '+08:00');

  const newMessage = {
    id: Date.now(),
    conversation_id: conversation_id.value,
    content: messageContent,
    type: 'text',
    time: formattedTime,
    sender: current_user_id.value,
    current_user_id: current_user_id.value,
  };

  displayMessage(newMessage);

  try {
    const response = await apiService.sendMessage(newMessage);
    if (response) {
      updateDisplayedMessage(newMessage.id, response.message_id);
    }
  } catch (error) {
    message.error('消息发送失败，请稍后重试');
  }
};

const displayMessage = (message) => {
  messages.value.push(message);
};

const updateDisplayedMessage = (oldId, newId) => {
  const oldMessage = messages.value.find(msg => msg.id === oldId);
  if (oldMessage) oldMessage.id = newId;
};

const isSelf = (sender_id) => sender_id === current_user_id.value;

const updateReadStatus = async () => {
  try {
    await apiService.updateReadStatus(conversation_id.value, current_user_id.value);
  } catch (e) {
    message.error(`更新已读状态失败: ${e}`);
  }
};

const retractMessage = async (message_id) => {
  try {
    await apiService.retractMessage(message_id, current_user_id.value);
    removeMessage(message_id);
  } catch (e) {
    message.error('撤回失败，请重试');
  }
};

const removeMessage = (message_id: number) => {
  messages.value = messages.value.filter(msg => msg.id !== message_id);
};

const connect = () => {
  socket.value = new WebSocket(`ws://121.36.200.128:5001/ws?user_id=${current_user_id.value}`);

  socket.value.onopen = () => {
    connectionStatus.value = 'Connected';
    message.success('WebSocket 连接成功');
  };

  socket.value.onclose = () => {
    connectionStatus.value = 'Disconnected';
    message.warning('WebSocket 连接已断开');
  };

  socket.value.onerror = (error) => {
    message.error(`WebSocket 错误: ${error.message}`);
  };

  socket.value.onmessage = (event) => {
    receiveMessage(event.data);
  };
};

const convertMessageFormat = (originalMessage) => {
  const msg = JSON.parse(originalMessage);
  return {
    id: msg.Message_ID,
    conversation_id: conversation_id.value,
    content: msg.Message_Content,
    type: msg.Message_Type,
    time: msg.Send_Time,
    sender: msg.Sender_User_ID,
    current_user_id: msg.Receiver_User_ID
  };
};

const receiver_in_window = ref(true);
const receiveMessage = (message) => {
  const newMessage = convertMessageFormat(message);
  if (newMessage.type === 'retract') {
    removeMessage(newMessage.id);
  } else if (newMessage.type === 'text') {
    displayMessage(newMessage);
  }
  apiService.receiveMessage(newMessage.id, receiver_in_window.value, newMessage.sender);
};

const disconnect = () => {
  if (socket.value) {
    socket.value.close();
    socket.value = null;
  }
};

const checkConnection = () => {
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
  message.info(`当前连接状态: ${connectionStatus.value}`);
};

const scrollToBottom = () => {
  nextTick(() => {
    if (chatContainerRef.value) {
      chatContainerRef.value.scrollTop = chatContainerRef.value.scrollHeight;
    }
  });
};

const checkFollowingStatus = async () => {
  try {
    const res = await axios.post("/api/user/follow/status", {
      target_id: conversation_id.value,
      current_user_id: current_user_id.value
    });
    is_following.value = res.data.is_following;
  } catch (err) {
    console.error(err);
    message.error('检查关注状态失败');
  }
};

onMounted(() => {
  updateReadStatus();
  getMessages();
  scrollToBottom();
  receiver_in_window.value = true;
  connect();
  checkFollowingStatus();
});

onUpdated(() => {
  scrollToBottom();
});

onUnmounted(() => {
  receiver_in_window.value = false;
  disconnect();
});
</script>

<style scoped>
.chat-container {
  height: calc(100vh - 225px);
  display: flex;
  flex-direction: column;
}

.messages-list {
  flex-grow: 1;
  overflow-y: auto;
  padding: 20px;
  background-color: #ffffff;
}

.message-input-container {
  padding: 10px;
  background-color: #fff;
  border-top: 1px solid #e8e8e8;
}

.connection-check-btn {
  margin-bottom: 10px;
}

.message-input {
  bottom: 50px;
  margin-right: 3vw;
  width: 100%;
}
</style>