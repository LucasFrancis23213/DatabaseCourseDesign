<script lang="ts" setup>
import { defineProps, computed ,ref, defineEmits, onMounted,onUnmounted} from 'vue'

const props = defineProps({
  content: {//聊天内容字符串
    type: String,
    required: true
  },
  time: {//消息发送时间
    type: String,
    default: ''
  },
  isSelf: {//是否为本人发送
    type: Boolean,
    default: false
  },
  avatar: {//头像URL地址
    type: String,
    default: ''
  },
  type:{//聊天内容类型
    type: String,
    default: "text"
  },
  id:{
    type: String,
    required:true
  },

});

const emit = defineEmits(['retract']);

function formatTime(timeString) {
  /*消息发送时间格式化
  * args:
  *   timeString:时间戳
  * */
  const now = new Date();
  const time = new Date(timeString);
  const diff = now - time;
  const minutes = Math.floor(diff / 60000);
  const hours = Math.floor(diff / 3600000);
  const days = Math.floor(diff / 86400000);

  if (minutes < 60) {
    return `${minutes}分钟前`;
  } else if (hours < 24) {
    return time.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit', second: '2-digit' });
  } else if (days === 1) {
    return '昨天';
  } else if (days < 7) {
    return `${days}天前`;
  } else {
    return '7天前';
  }
}


const showRetract = ref(false); // 是否显示撤回按钮
const retractMessage = (()=>{
  emit('retract',props.id);
});

const toggleRetract = ()=>{
  showRetract.value=!showRetract.value;
};

</script>

<template>
  <div class="chat-bubble" :class="{ 'self': isSelf }">
    <div class="avatar" v-if="!isSelf">
      <img :src="avatar" alt="User Avatar">
    </div>
    <div class="message-container" @mouseenter="toggleRetract" @mouseleave="toggleRetract">
      <div class="message"  >{{ content }}</div>
      <div v-if="showRetract && props.isSelf" class="retract-action">
        <button @click="retractMessage">撤回</button>
      </div>
      <div class="time">{{ formatTime(time) }}</div>
    </div>
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

.message-container {
  display: flex;
  flex-direction: column;
  max-width: 70%;
}

.message {
  background-color: #f1f0f0;
  border-radius: 18px;
  padding: 10px 15px;
  word-wrap: break-word;
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

.self .message-container {
  align-items: flex-end;
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