<template>
  <div class="conversation-list">
    <h1>消息列表</h1>
<!--    <a-input-->
<!--      v-model:value="userId"-->
<!--      placeholder="想要建立会话的用户id"-->
<!--      @pressEnter="navigateToConversation(userId, currentUserId)"-->
<!--    >-->
<!--      <template #suffix>-->
<!--        <a-button type="primary" @click="navigateToConversation(userId, currentUserId)">确定</a-button>-->
<!--      </template>-->
<!--    </a-input>-->
    <div
      v-for="conversation in conversations"
      :key="conversation.id"
      class="conversation-item"
      @click="navigateToConversation(conversation, currentUserId)"
    >
      <a-avatar :src="conversation.avatar" :alt="conversation.name" class="avatar" />
      <div class="conversation-details">
        <div class="name-and-time">
          <span class="name">{{ conversation.name }}</span>
          <span class="time">{{ formatTime(conversation.last_message_time) }}</span>
        </div>
        <div class="message">{{ format_last_message(conversation.last_message,conversation.id) }}</div>
      </div>
      <a-badge
        v-if="conversation.unread_count > 0"
        :count="conversation.unread_count"
        class="unread-badge"
      />
    </div>
  </div>
</template>

<script setup>
import {ref, onMounted, computed} from 'vue';
import { useRouter } from 'vue-router';
import { message } from 'ant-design-vue';
import axios from 'axios';
import { useAccountStore } from '@/store/account';

const { account } = useAccountStore();
const router = useRouter();

const conversations = ref([]);
const userId = ref('');
const currentUserId = ref(account.userId);

axios.defaults.baseURL = import.meta.env.VITE_API_URL;

const getConversations = async () => {
  try {
    const res = await axios.post('/api/conversations', { current_user_id: currentUserId.value });
    conversations.value = res.data.conversations;
  } catch (e) {
    console.error(e);
    message.error(`获取消息列表失败，错误信息为：${e.message}`);
  }
};

onMounted(getConversations);

const formatTime = (timeString) => {
  const date = new Date(timeString);
  const now = new Date();
  const diffDays = Math.floor((now - date) / (1000 * 60 * 60 * 24));

  if (diffDays === 0) {
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  } else if (diffDays === 1) {
    return '昨天';
  } else if (diffDays < 7) {
    return ['周日', '周一', '周二', '周三', '周四', '周五', '周六'][date.getDay()];
  } else {
    return date.toLocaleDateString();
  }
};

const navigateToConversation = (conversation, currentUserId) => {
  router.push({
    name: '聊天',
    params: { conversation_id: conversation.id },
    query: { current_user_id: currentUserId },
  });
};

const isSystemMsg=(sender) => {
  return sender === +import.meta.env.VITE_SYSTEM_USER_ID
};
function format_last_message(last_message,sender){
  if(isSystemMsg(sender)) {
    return JSON.parse(last_message).content;
  }
  else
    return last_message;
}

</script>

<style scoped>
.conversation-list {

  margin: 0 auto;
  font-family: 'Arial', sans-serif;
  background-color: #f5f5f5;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

h1 {
  text-align: center;
  color: #333;
  padding: 20px 0;
  background-color: #fff;
  margin: 0;
  border-bottom: 1px solid #eee;
}

.conversation-item {
  display: flex;
  align-items: center;
  padding: 15px;
  border-bottom: 1px solid #eee;
  position: relative;
  cursor: pointer;
  transition: background-color 0.3s ease;
  background-color: #fff;
}

.conversation-item:hover {
  background-color: #f0f0f0;
}

.conversation-item:last-child {
  border-bottom: none;
}

.avatar {
  margin-right: 15px;
  transition: transform 0.2s ease;
}

.conversation-item:hover .avatar {
  transform: scale(1.05);
}

.conversation-details {
  flex-grow: 1;
  min-width: 0;
}

.name-and-time {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 5px;
}

.name {
  font-weight: bold;
  color: #333;
  font-size: 1.1em;
}

.time {
  font-size: 0.8em;
  color: #888;
  margin-right: 25px;
}

.message {
  color: #666;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  font-size: 0.9em;
  max-width: 80%;
}

.unread-badge {
  position: absolute;
  top: 50%;
  right: 15px;
  transform: translateY(-50%);
  transition: transform 0.2s ease;
}

.conversation-item:hover .unread-badge {
  transform: translateY(-50%) scale(1.1);
}

@media (max-width: 480px) {
  .conversation-list {
    border-radius: 0;
    box-shadow: none;
  }

  .name {
    font-size: 1em;
  }

  .message {
    font-size: 0.85em;
  }
}
</style>