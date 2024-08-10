<template>
  <button
      @click="toggleFollow"
      :class="{ 'followed': isFollowed }"
  >
    {{ buttonText }}
  </button>
</template>

<script setup>
import {ref, computed} from 'vue'
import axios from "axios";
axios.defaults.baseURL = import.meta.env.VITE_API_URL;
import {unfollowUser} from "@/components/CommunityFeature/follow/followApi.ts";
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

const emit = defineEmits(['follow', 'unfollow'])

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
  emit(isFollowed.value ? 'follow' : 'unfollow')
}

const followUser = async (followUserId) => {
  try{
    const res = await axios.post("api/user/follow",{
      user_id:followUserId,
      action:"unfollow",
      current_user_id:account.userId
    })
  }catch (error){
    console.error(`关注失败，错误原因${error}`);
    alert(`关注失败，请重试`);
  }
}
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