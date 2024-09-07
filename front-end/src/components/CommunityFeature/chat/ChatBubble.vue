<script lang="ts" setup>
import {  computed ,ref, onMounted,onUnmounted} from 'vue'
import { useTimeFormat } from './useTimeFormat';
import followButton from '@/components/CommunityFeature/follow/followButton.vue'
import CreateConversationBtn from "@/components/CommunityFeature/chat/CreateConversationBtn.vue";
import { useAccountStore } from '@/store/account';

const { account } = useAccountStore();
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
  message_id:{
    type: Number,
    required:true
  },
  user_name:{
    type: String,
    default:''
  },
  message_sender_id:{
    type: Number,
    required:true
  },
  is_following:{
    type:Boolean,
    required:true
  }

});
const emit = defineEmits(['retract','follow']);
const { formattedTime } = useTimeFormat(props.time);
const showRetract = ref(false); // 是否显示撤回按钮
const retractMessage = (()=>{
  emit('retract',props.message_id);
});
const toggleRetract = ()=>{
  showRetract.value=!showRetract.value;
};
const currentTime = ref(Date.now());//当前时间

onMounted(() => {
  const timer = setInterval(() => {
    currentTime.value = Date.now();
  }, 1000); // 每秒更新一次

  onUnmounted(() => {
    clearInterval(timer);
  });
});

const displayAvatar = computed(() => props.avatar || account.avatar);

const isWithinFiveMinutes = computed(() => {
  const messageTime = new Date(props.time).getTime();
  const fiveMinutesInMs = 5 * 60 * 1000;
  return currentTime.value - messageTime <= fiveMinutesInMs;
});

const showUserInfo = ref(false);
const toggleUserInfo = () => {
  showUserInfo.value = !showUserInfo.value;
};

const isSystemMsg=computed(() => {
  return props.message_sender_id === +import.meta.env.VITE_SYSTEM_USER_ID
});

const Content = computed(()=>{
  if(isSystemMsg.value) {
    return JSON.parse(props.content).content;
  }
  else
    return props.content;
})

const build_chat_user_id = computed(()=>{
  if(isSystemMsg.value){
    return JSON.parse(props.content).build_chat_user_id;
  }
  else
    return undefined;
});
const handleOk = () => {
  showUserInfo.value = false;
};

const handleCancel = () => {
  showUserInfo.value = false;
};

const modalVisible = computed({
  get: () => showUserInfo.value && ! props.isSelf.value,
  set: (value) => {
    showUserInfo.value = value;
  }
});
//console.log(account);

</script>

<template>
  <div class="chat-bubble" :class="{ 'self': isSelf }">
    <div class="avatar" @click="toggleUserInfo" ref="avatarRef">
      <img :src="displayAvatar" alt="User Avatar">
    </div>
    <div class="message-container" @mouseenter="toggleRetract" @mouseleave="toggleRetract">
      <div class="message"  >{{ Content }}</div>


      <div v-if="showRetract && props.isSelf && isWithinFiveMinutes" class="retract-action">
        <button @click="retractMessage">撤回</button>
      </div>
      <div class="time">{{ formattedTime }}</div>
      <CreateConversationBtn v-if="isSystemMsg&&!isSelf" :target-id="build_chat_user_id" ></CreateConversationBtn>
    </div>
    <a-modal v-model:visible="modalVisible" title="用户详情" @ok="handleOk" @cancel="handleCancel">
      <div v-if="showUserInfo ">
        <p><strong>ID:</strong> {{ message_sender_id }}</p>
        <p><strong>用户名:</strong> {{ user_name }}</p>
        <followButton class="follow-button" :user_id="message_sender_id" :initial-follow-state="is_following" ></followButton>
      </div>
    </a-modal>
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
button {
  display: inline-block;
  padding: 5px 7px;
  font-size: 10px;
  font-weight: bold;
  text-align: center;
  text-decoration: none;
  color: #ffffff;
  background-color: #007bff;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

button:hover {
  background-color: #0056b3;
}

button:active {
  background-color: #004085;
}

.user-info {
  position: absolute;
  left: 50px;
  top: 0;
  background-color: white;
  border: 1px solid #ccc;
  border-radius: 8px;
  padding: 10px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  z-index: 10;
}
</style>