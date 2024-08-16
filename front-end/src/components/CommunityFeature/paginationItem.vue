<template>
  <div class="pagination">
    <button @click="$emit('prev')" :disabled="currentPage === 1">上一页</button>
    <span>{{ currentPage }} / {{ totalPages }}</span>
    <button @click="$emit('next')" :disabled="currentPage === totalPages">下一页</button>
  </div>
</template>

<script setup>
import { defineProps, defineEmits } from 'vue';

const props = defineProps({
  currentPage: {
    type: Number,
    required: true
  },
  totalPages: {
    type: Number,
    required: true
  }
});

defineEmits(['prev', 'next']);
</script>

<style scoped>
.pagination {
  margin-top: 20px;
  display: flex;
  justify-content: center;
  align-items: center;
}
.pagination button {
  margin: 0 10px;
}
</style>