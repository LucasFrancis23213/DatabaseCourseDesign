<!--<script lang="ts" setup>-->
<!--import { ref } from 'vue';-->

<!--const message = ref('');-->
<!--const messages = ref([]);-->
<!--const isSending = ref(false);-->

<!--function sendMessage() {-->
<!--  if (message.value.trim() && !isSending.value) {-->
<!--    isSending.value = true;-->

<!--    // 模拟发送消息的延迟-->
<!--    setTimeout(() => {-->
<!--      // 添加新消息到消息列表-->
<!--      messages.value.push({-->
<!--        id: Date.now(),-->
<!--        content: message.value,-->
<!--        type: 'sent',-->
<!--        time: new Date().toLocaleTimeString()-->
<!--      });-->

<!--      message.value = ''; // 清空输入框-->
<!--      isSending.value = false;-->
<!--    }, 500); // 500ms 的模拟延迟-->
<!--  }-->
<!--}-->
<!--</script>-->

<!--<template>-->
<!--  <div class="chat-container">-->
<!--    <div class="messages-list">-->
<!--      <div-->
<!--          v-for="msg in messages"-->
<!--          :key="msg.id"-->
<!--          :class="['message', msg.type]">-->
<!--        {{ msg.content }}-->
<!--        <span class="time">{{ msg.time }}</span>-->
<!--      </div>-->
<!--    </div>-->

<!--    <div class="message-input">-->
<!--      <input v-model="message" placeholder="输入消息" @keyup.enter="sendMessage"/>-->
<!--      <button @click="sendMessage" :disabled="!message.trim() || isSending">-->
<!--        {{ isSending ? '发送中...' : '发送' }}-->
<!--      </button>-->
<!--    </div>-->
<!--  </div>-->
<!--</template>-->

<!--<style scoped>-->
<!--.chat-container {-->
<!--  display: flex;-->
<!--  flex-direction: column;-->
<!--  height: 100vh;-->
<!--  padding: 20px;-->
<!--  box-sizing: border-box;-->
<!--}-->

<!--.messages-list {-->
<!--  flex-grow: 1;-->
<!--  overflow-y: auto;-->
<!--  margin-bottom: 20px;-->
<!--}-->

<!--.message {-->
<!--  max-width: 70%;-->
<!--  margin-bottom: 10px;-->
<!--  padding: 10px;-->
<!--  border-radius: 10px;-->
<!--  word-wrap: break-word;-->
<!--}-->

<!--.message.sent {-->
<!--  background-color: #dcf8c6;-->
<!--  align-self: flex-end;-->
<!--  margin-left: auto;-->
<!--}-->

<!--.time {-->
<!--  font-size: 0.8em;-->
<!--  color: #888;-->
<!--  display: block;-->
<!--  text-align: right;-->
<!--  margin-top: 5px;-->
<!--}-->

<!--.message-input {-->
<!--  display: flex;-->
<!--  margin-top: 10px;-->
<!--}-->

<!--input {-->
<!--  flex-grow: 1;-->
<!--  padding: 10px;-->
<!--  border: 1px solid #ccc;-->
<!--  border-radius: 4px 0 0 4px;-->
<!--  font-size: 16px;-->
<!--}-->

<!--button {-->
<!--  padding: 10px 20px;-->
<!--  background-color: #4CAF50;-->
<!--  color: white;-->
<!--  border: none;-->
<!--  border-radius: 0 4px 4px 0;-->
<!--  cursor: pointer;-->
<!--  font-size: 16px;-->
<!--}-->

<!--button:disabled {-->
<!--  background-color: #cccccc;-->
<!--  cursor: not-allowed;-->
<!--}-->
<!--</style>-->

<script lang="ts" setup>
import { ref, computed } from 'vue';

const props = defineProps({
  isSending: {
    type: Boolean,
    default: false
  }
});

const emit = defineEmits(['send-message']);

const message = ref('');

const isMessageEmpty = computed(() => message.value.trim().length === 0);

function sendMessage() {
  if (!isMessageEmpty.value && !props.isSending) {
    emit('send-message', message.value.trim());
    message.value = '';
  }
}

function adjustTextareaHeight(event: Event) {
  const textarea = event.target as HTMLTextAreaElement;
  textarea.style.height = 'auto';
  textarea.style.height = `${textarea.scrollHeight}px`;
}

function addEmoji(emoji: string) {
  message.value += emoji;
}
</script>

<template>
  <div class="message-input">
    <button class="emoji-button" @click="addEmoji('😊')">😊</button>
    <textarea
      v-model="message"
      placeholder="输入消息"
      @input="adjustTextareaHeight"
      @keyup.enter.exact.prevent="sendMessage"
      @keyup.enter.shift.exact.prevent="message += '\n'"
    ></textarea>
    <button
      @click="sendMessage"
      :disabled="isMessageEmpty || isSending"
      :class="{ 'sending': isSending }"
    >
      {{ isSending ? '发送中...' : '发送' }}
    </button>
  </div>
</template>

<style scoped>
.message-input {
  display: flex;
  align-items: flex-end;
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  padding: 10px;
}

.emoji-button {
  padding: 5px 10px;
  font-size: 20px;
  background: none;
  border: none;
  cursor: pointer;
}

textarea {
  flex-grow: 1;
  padding: 10px;
  border: 1px solid #ccc;
  border-radius: 4px 0 0 4px;
  font-size: 16px;
  resize: none;
  min-height: 40px;
  max-height: 120px;
  overflow-y: auto;
}

button {
  padding: 10px 20px;
  background-color: #4CAF50;
  color: white;
  border: none;
  border-radius: 0 4px 4px 0;
  cursor: pointer;
  font-size: 16px;
  transition: background-color 0.3s;
}

button:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}

button.sending {
  background-color: #45a049;
}
</style>