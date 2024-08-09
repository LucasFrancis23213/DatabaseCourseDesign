<template>
  <div class="fans-list">
    <h2>我的粉丝</h2>
    <div v-if="loading">加载中...</div>
    <div v-else-if="error">{{ error }}</div>
    <template v-else>
      <ul v-if="followers.length">
        <li v-for="follower in followers" :key="follower.user_id">
          <img :src="follower.user_avatar" :alt="follower.user_name">
          <span>{{ follower.user_name }}</span>
        </li>
      </ul>
      <p v-else>暂无粉丝</p>

      <div class="pagination">
        <button @click="fetchFollowers(currentPage - 1)" :disabled="currentPage === 1">上一页</button>
        <span>{{ currentPage }} / {{ totalPages }}</span>
        <button @click="fetchFollowers(currentPage + 1)" :disabled="currentPage === totalPages">下一页</button>
      </div>
    </template>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import axios from 'axios'
axios.defaults.baseURL = import.meta.env.VITE_API_URL;
import { useAccountStore } from '@/store/account';
const {account, permissions} = useAccountStore();


const followers = ref([])
const currentPage = ref(1)
const totalPages = ref(1)
const loading = ref(false)
const error = ref(null)
const itemsPerPage = 10

const fetchFollowers = async (page = 1) => {
  loading.value = true
  error.value = null
  try {
    const response = await axios.post('/api/user/followers', {
      user_id: account.userId, // 假设当前用户ID为1，实际应用中应该是动态的
    })

    if (response.data.status === 'success') {
      followers.value = response.data.followers
      totalPages.value = Math.ceil(response.data.total_count / itemsPerPage)
    } else {
      throw new Error(response.data.status)
    }
  } catch (err) {
    error.value = '获取粉丝列表失败: ' + err.message
  } finally {
    loading.value = false
  }
}

onMounted(() => fetchFollowers())
</script>

<style scoped>
.fans-list {
  /* 添加样式 */
}

.pagination {
  margin-top: 20px;
}

ul {
  list-style-type: none;
  padding: 0;
}

li {
  display: flex;
  align-items: center;
  margin-bottom: 10px;
}

li img {
  width: 50px;
  height: 50px;
  border-radius: 50%;
  margin-right: 10px;
}

/* 添加更多样式... */
</style>