<template>
  <button
      @click="toggleFollow"
      :class="{ 'followed': isFollowed }"
  >
    {{ buttonText }}
  </button>
</template>

<script setup>
import {ref, computed, onMounted} from 'vue'
import axios from "axios";
axios.defaults.baseURL = import.meta.env.VITE_API_URL;
import { useAccountStore } from '@/store/account';
const {account, permissions} = useAccountStore();

const props = defineProps({
  initialFollowState: {
    type: Boolean,
    default: false
  },
  followedText: {
    type: String,
    default: '已关注'
  },
  unfollowedText: {
    type: String,
    default: '关注'
  },
  user_id:{
    type:Number,
    required:true
  }

})

const emit = defineEmits(['follow'])

const isFollowed = ref(props.initialFollowState)

const buttonText = computed(() => {
  return isFollowed.value ? props.followedText : props.unfollowedText
})

const toggleFollow = () => {
  if (!isFollowed.value) {
    followUser(props.user_id);
  }else{
    unfollowUser(props.user_id);
  }
  isFollowed.value = !isFollowed.value

}

const followUser = async (followUserId) => {
  try{
    const res = await axios.post("api/user/follow",{
      user_id:followUserId,
      action:"follow",
      current_user_id:account.userId
    })
  }catch (error){
    console.error(error);
    alert(`关注失败，请重试`);
  }
}
const unfollowUser = async (unfollowUserId) => {
  try {
    await axios.post(`/api/user/follow`,{
      user_id:unfollowUserId,
      action:"unfollow",
      current_user_id:account.userId
    })
  } catch (error) {
    console.error('取消关注失败:', error)
    throw error
  }
}
const checkFollowingStatus = async ()=>{
  try {
    const res = await axios.post("/api/user/follow/status", {
      target_id: props.user_id,
      current_user_id: account.userId
    })

    isFollowed.value=res.data.is_following

  }catch (err){
    console.error(err)
  }

}

onMounted(()=>{
  checkFollowingStatus();
});
</script>

<style scoped>
button {
  padding: 8px 16px;
  border: 1px solid #ccc;
  border-radius: 4px;
  cursor: pointer;
  transition: all 0.3s ease;
}

button.followed {
  background-color: #1da1f2;
  color: white;
}
</style>