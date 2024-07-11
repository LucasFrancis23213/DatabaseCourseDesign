<script setup>
import ChatBubble from "@/components/BasicFeature/ChatBubble.vue";
import { ref } from 'vue'
import MessageInput from './MessageInput.vue';

const messages = ref([]);

const handleSendMessage = (newMessage) => {
  messages.value.push({
    id: Date.now(),
    text: newMessage,
    time: new Date().toLocaleTimeString(),
    isSelf: true
  });
  // 这里可以添加发送消息到服务器的逻辑
};

</script>

<template>
  <div class="chat-container">
    <ChatBubble
        v-for="(msg, index) in messages"
        :key="index"
        :message="msg.text"
        :time="msg.time"
        :isSelf="msg.isSelf"
        :avatar="msg.avatar"
    />
    <div class="chat-app">
    <!-- 聊天气泡组件们 -->
    <MessageInput @send-message="handleSendMessage" />
  </div>
  </div>
</template>

<style scoped>
.chat-container{
  background-color: #776f6f;
}
</style>