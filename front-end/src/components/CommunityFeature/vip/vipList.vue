<template>
  <div class="vip-user-list">
    <h2>VIP用户列表</h2>
    <ul>
      <li v-for="user in paginatedUsers" :key="user.id">
        {{ user.name }} - VIP等级: {{ user.vipLevel }}
      </li>
    </ul>
    <div class="pagination">
      <button @click="prevPage" :disabled="currentPage === 1">上一页</button>
      <span>{{ currentPage }} / {{ totalPages }}</span>
      <button @click="nextPage" :disabled="currentPage === totalPages">下一页</button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'

// 模拟API调用获取用户数据
const fetchUsers = async () => {
  // 这里应该是实际的API调用
  return [
    { id: 1, name: '用户1', vipLevel: 1 },
    { id: 2, name: '用户2', vipLevel: 2 },
    // ... 更多用户数据
  ]
}

const users = ref([])
const currentPage = ref(1)
const itemsPerPage = 10

const totalPages = computed(() => Math.ceil(users.value.length / itemsPerPage))

const paginatedUsers = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage
  const end = start + itemsPerPage
  return users.value.slice(start, end)
})

const nextPage = () => {
  if (currentPage.value < totalPages.value) {
    currentPage.value++
  }
}

const prevPage = () => {
  if (currentPage.value > 1) {
    currentPage.value--
  }
}

onMounted(async () => {
  users.value = await fetchUsers()
})
</script>