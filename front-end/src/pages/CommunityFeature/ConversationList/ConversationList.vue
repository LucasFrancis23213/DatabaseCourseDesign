<template>
  <div class="conversation-list">
    <h1>用户列表</h1>
    <a-input
      v-model:value="userId"
      placeholder="想要建立会话的用户id"
      @pressEnter="navigateToConversation(userId, currentUserId)"
    >
      <template #suffix>
        <a-button type="primary" @click="navigateToConversation(userId, currentUserId)">确定</a-button>
      </template>
    </a-input>
    <div
      v-for="conversation in conversations"
      :key="conversation.id"
      class="conversation-item"
      @click="navigateToConversation(conversation.id, currentUserId)"
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

const navigateToConversation = (conversationId, currentUserId) => {
  router.push({
    name: '聊天',
    params: { conversation_id: conversationId },
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
  max-width: 600px;
  margin: 0 auto;
  font-family: Arial, sans-serif;
}

h1 {
  text-align: center;
  color: #333;
}

.conversation-item {
  display: flex;
  align-items: center;
  padding: 15px;
  border-bottom: 1px solid #eee;
  position: relative;
  cursor: pointer;
}

.avatar {
  margin-right: 15px;
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
}

.unread-badge {
  position: absolute;
  top: 15px;
  right: 15px;
}
</style>