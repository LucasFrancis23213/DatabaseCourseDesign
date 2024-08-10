<template>
  <div class="following-list">
    <h2>关注列表</h2>
    <ul v-if="following.length > 0">
      <li v-for="user in following" :key="user.user_id" class="following-item">
        <img :src="user.user_avatar" :alt="user.user_name" class="avatar">
        <span class="username">{{ user.user_name }}</span>
        <button @click="unfollow(user.user_id)" class="unfollow-btn">取消关注</button>
      </li>
    </ul>
    <p v-else>您还没有关注任何人。</p>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import axios from 'axios'
axios.defaults.baseURL = import.meta.env.VITE_API_URL;
import { useAccountStore } from '@/store/account';
const {account, permissions} = useAccountStore();




const following = ref([])

const fetchFollowing = async () => {
  try {
    following.value = await getFollowing();
  } catch (error) {
    console.error('获取关注列表失败:', error);
    alert(`获取关注列表失败，错误原因：${error}`);
  }
}

const unfollow = async (userId) => {
  try {
    await unfollowUser(userId);
    following.value = following.value.filter(user => user.user_id !== userId)
  } catch (error) {
    console.error('取消关注失败:', error)
    alert(`取消关注失败，错误原因：${error}`);
  }
}


const getFollowing = async () => {
  try {
    const response = await axios.post('/api/user/following',{
      user_id:account.userId
    })
    return response.data.following
  } catch (error) {
    console.error('获取关注列表失败:', error)
    throw error
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

onMounted(fetchFollowing)
</script>

<style scoped>
.following-list {
  /* 添加样式 */
}
.following-item {
  display: flex;
  align-items: center;
  margin-bottom: 10px;
}
.avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  margin-right: 10px;
}
.username {
  flex-grow: 1;
}
.unfollow-btn {
  /* 按钮样式 */
}
</style>