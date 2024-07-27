<script setup>
import { defineProps } from 'vue'

defineProps({
  message: {
    type: String,
    required: true
  },
  time: {
    type: String,
    default: ''
  },
  isSelf: {
    type: Boolean,
    default: false
  },
  avatar: {
    type: String,
    default: ''
  }
})
</script>

<template>
  <div class="chat-bubble" :class="{ 'self': isSelf }">
    <div class="avatar" v-if="!isSelf">
      <img :src="avatar" alt="User Avatar">
    </div>
    <div class="message">
      {{ message }}
    </div>
    <div class="time">{{ time }}</div>
  </div>
</template>

<style scoped>
.chat-bubble {
  display: flex;
  margin-bottom: 15px;
  align-items: flex-start;
}

.avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  overflow: hidden;
  margin-right: 10px;
}

.avatar img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.message {
  background-color: #f1f0f0;
  border-radius: 18px;
  padding: 10px 15px;
  max-width: 70%;
}

.time {
  font-size: 0.75em;
  color: #888;
  align-self: flex-end;
  margin-left: 10px;
}

.self {
  flex-direction: row-reverse;
}

.self .message {
  background-color: #dcf8c6;
}

.self .avatar {
  margin-right: 0;
  margin-left: 10px;
}

.self .time {
  margin-left: 0;
  margin-right: 10px;
}
</style>