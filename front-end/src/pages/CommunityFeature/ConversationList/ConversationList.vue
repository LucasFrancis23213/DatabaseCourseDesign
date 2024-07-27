<template>
  <div class="conversation-list">
    <h1>用户列表</h1>
    <div v-for="conversation in conversations"
         :key="conversation.id"
         class="conversation-item"
         @click="navigateToConversation(conversation.id,current_user_id)"
    >
      <img :src="conversation.avatar" :alt="conversation.name" class="avatar">
      <div class="conversation-details">
        <div class="name-and-time">
          <span class="name">{{ conversation.name }}</span>
          <span class="time">{{ formatTime(conversation.last_message_time) }}</span>
        </div>
        <div class="message">{{ conversation.last_message }}</div>
      </div>
      <div v-if="conversation.unread_count > 0" class="unread-badge">
        {{ conversation.unread_count }}
      </div>
    </div>
  </div>
</template>

<script setup>

import {onMounted, ref} from "vue";
import {useRouter} from "vue-router";
import axios from 'axios';
const BaseURL = import.meta.env.VITE_API_URL;

let conversations = ref([]);
const router = useRouter();
let current_user_id = ref("12345");
async function getConversations() {
  console.log("getConversations");
  try {
    //const res = await axios.get(`${BaseURL}/api/conversations`);
    const res = await axios.post(`${BaseURL}/api/conversations`,{current_user_id:current_user_id.value})
    console.log(res);
    conversations.value = res.data.conversations;
    //console.log("conversations are " + conversations);
  } catch (err) {
    console.log(err);
  }
}

onMounted(()=>{
  getConversations();
})

const formatTime = (timeString) => {
  const date = new Date(timeString)
  const now = new Date()
  const diffDays = Math.floor((now - date) / (1000 * 60 * 60 * 24))

  if (diffDays === 0) {
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })
  } else if (diffDays === 1) {
    return '昨天'
  } else if (diffDays < 7) {
    return ['周日', '周一', '周二', '周三', '周四', '周五', '周六'][date.getDay()]
  } else {
    return date.toLocaleDateString()
  }
}

console.log(current_user_id.value);
const navigateToConversation = (conversation_id,current_user_id) => {
  console.log(conversation_id,current_user_id);
  router.push({
    name: "聊天",
    params: {
      "conversation_id": conversation_id ,

    },
    // 如果你想传递额外的数据，可以使用 query 参数
    query: { "current_user_id":current_user_id, }
  })
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
  width: 50px;
  height: 50px;
  border-radius: 50%;
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
  background-color: #ff4136;
  color: white;
  border-radius: 50%;
  min-width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.8em;
  position: absolute;
  top: 15px;
  right: 15px;
}
</style>