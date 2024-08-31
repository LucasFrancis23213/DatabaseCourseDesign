<template>
  <div class="bg-container shadow-middle rounded-md p-md">
    <h2 class="text-lg text-title font-bold mb-md text-center">我的关注</h2>
    <transition-group name="list" tag="ul" v-if="following.length > 0" class="space-y-sm">
      <li v-for="user in displayedFilteredFollowing" :key="user.user_id" class="flex items-center p-sm bg-container-light rounded-sm hover:bg-bg-hover">
        <img :src="user.user_avatar" :alt="user.user_name" class="w-10 h-10 rounded-full mr-sm">
        <div class="flex-grow flex justify-between items-center">
          <span class="text-text font-medium">{{ user.user_name }}</span>
          <button @click="unfollow(user.user_id)" class="text-sm bg-error-bg text-error-text hover:bg-error-bg-hover px-sm py-xxs rounded-md transition-colors duration-200">
            <span class="mr-xxs">取消关注</span>
            <span class="font-bold">×</span>
          </button>
        </div>
      </li>

    </transition-group>
    <p v-else class="text-text-3 text-center p-md">您还没有关注任何人。</p>
    <div class="flex justify-center items-center mt-xl">

        <a-pagination
    :total="following.length"
    :current="currentPage"
    :pageSize="pageSize"
    @change="onPageChange"
  />
      </div>
  </div>
</template>
<script setup>
import {ref, onMounted, computed} from 'vue'
import axios from 'axios'
axios.defaults.baseURL = import.meta.env.VITE_API_URL;
import { useAccountStore } from '@/store/account';
import {StepinHeaderAction, StepinLink, StepinView} from "stepin";
const {account, permissions} = useAccountStore();
import { ThemeProvider } from 'stepin/es/theme-provider'
import {useThemeStore}  from "stepin/es/theme-provider";
const { setPrimaryColor } = useThemeStore();
setPrimaryColor({DEFAULT: '#3B82F6'});

const following = ref([])
const pageSize = ref(10)
const currentPage = ref(1)

const onPageChange = (page, size) => {
  currentPage.value = page;
  pageSize.value = size;
};
const displayedFilteredFollowing = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value;
  const end = start + pageSize.value;
  return following.value.slice(start, end);
});

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
    console.log(response);
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

</style>