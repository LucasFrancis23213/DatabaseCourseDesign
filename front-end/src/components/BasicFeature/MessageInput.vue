<script lang="ts" setup>
import {ref} from 'vue';

const message = ref('');
const isSending = ref(false);
const conversationId = 'CONV123'; // 这应该是动态的，根据当前会话来设置

async function sendMessageRequest(conversationId, content, type) {
  const response = await fetch('/api/conversations/messages', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      conversation_id: conversationId,
      content: content,
      type: type
    }),
  });

  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }

  const data = await response.json();

  if (data.status === 'success') {
    return {
      messageId: data.message_id,
      time: data.time
    };
  } else {
    throw new Error('消息发送失败');
  }
}

async function sendMessage() {
  if (message.value.trim() && !isSending.value) {
    isSending.value = true;
    try {
      const result = await sendMessageRequest(conversationId, message.value, 'text');
      console.log('消息发送成功:', result);
      message.value = ''; // 清空输入框
      // 这里可以更新UI，比如将新消息添加到消息列表
    } catch (error) {
      console.error('消息发送失败:', error);
      // 这里可以显示错误提示
    } finally {
      isSending.value = false;
    }
  }
}
</script>

<template>

  <div class="message-input">
    <input v-model="message" placeholder="输入消息"/>
    <button @click="sendMessage" :disabled="!message.trim() || isSending">
      {{ isSending ? '发送中...' : '发送' }}
    </button>
  </div>
</template>

<style scoped>
.message-input {
  display: flex;
  margin-top: 10px;
}

input {
  flex-grow: 1;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 4px 0 0 4px;
  font-size: 16px;
}

button {
  padding: 10px 20px;
  background-color: #4CAF50;
  color: white;
  border: none;
  border-radius: 0 4px 4px 0;
  cursor: pointer;
  font-size: 16px;
}

button:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}
</style>