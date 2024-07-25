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