<script lang="ts" setup>
import { ref } from 'vue';
import ChatBubble from './ChatBubble.vue';
import MessageInput from './MessageInput.vue';

interface Message {
  id: number;
  content: string;
  time: string;
  isSelf: boolean;
  avatar: string;
}

const messages = ref<Message[]>([]);
const isSending = ref(false);

function handleSendMessage(messageContent: string) {
  if (!isSending.value) {
    isSending.value = true;

    // 模拟发送消息的延迟
    setTimeout(() => {
      const newMessage: Message = {
        id: Date.now(),
        content: messageContent,
        time: new Date().toLocaleTimeString(),
        isSelf: true,
        avatar: 'path/to/user-avatar.jpg' // 替换为实际的用户头像路径
      };

      messages.value.push(newMessage);
      isSending.value = false;
    }, 500);
  }
}
</script>

<template>
  <div class="chat-container">
    <div class="messages-list">
      <ChatBubble
        v-for="msg in messages"
        :key="msg.id"
        :message="msg.content"
        :time="msg.time"
        :is-self="msg.isSelf"
        :avatar="msg.avatar"
      />
    </div>

    <MessageInput
      :is-sending="isSending"
      @send-message="handleSendMessage"
    />
  </div>
</template>

<style scoped>
.chat-container {
  display: flex;
  flex-direction: column;
  //height: 100vh;
  padding: 20px;
  box-sizing: border-box;
}

.messages-list {
  flex-grow: 1;
  overflow-y: auto;
  margin-bottom: 20px;
}
</style>