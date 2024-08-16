<template>
  <div class="bg-container shadow-middle rounded-md p-md">
    <h2 class="text-lg text-title font-bold mb-lg text-center">我的粉丝</h2>
    <div v-if="loading" class="text-text-3 text-center p-md italic">加载中...</div>
    <div v-else-if="error" class="text-error-500 text-center p-md italic">{{ error }}</div>
    <template v-else>
      <ul v-if="followers.length" class="space-y-sm">
        <li v-for="follower in followers" :key="follower.user_id"
            class="flex items-center p-sm bg-container-light rounded-sm hover:bg-bg-hover transition-colors duration-200">
          <img :src="follower.user_avatar" :alt="follower.user_name"
               class="w-12 h-12 rounded-full mr-sm object-cover">
          <span class="text-text font-medium">{{ follower.user_name }}</span>
        </li>
      </ul>
      <p v-else class="text-text-3 text-center p-md italic">暂无粉丝</p>

      <div class="flex justify-center items-center mt-xl">
        <button @click="fetchFollowers(currentPage - 1)"
                :disabled="currentPage === 1"
                class="bg-primary-bg text-primary-text hover:bg-primary-bg-hover disabled:bg-disabled disabled:text-text-disabled px-md py-sm rounded-md transition-colors duration-200 mr-sm">
          上一页
        </button>
        <span class="text-text-2 text-sm mx-sm">{{ currentPage }} / {{ totalPages }}</span>
        <button @click="fetchFollowers(currentPage + 1)"
                :disabled="currentPage === totalPages"
                class="bg-primary-bg text-primary-text hover:bg-primary-bg-hover disabled:bg-disabled disabled:text-text-disabled px-md py-sm rounded-md transition-colors duration-200 ml-sm">
          下一页
        </button>
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
      user_id: account.userId,
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

</style>