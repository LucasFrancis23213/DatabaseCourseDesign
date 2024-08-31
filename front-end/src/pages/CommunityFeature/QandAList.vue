<template>
  <div>
    <questions-list :item_id="itemId" :current_user="currentUser" />
  </div>
</template>

<script lang="ts" setup>
import QuestionsList from '../../components/CommunityFeature/QandAList.vue';
import { useAccountStore } from '@/store/account';
const {account,permissions} = useAccountStore();
// 示例用户数据
let currentUser = {
    id: account.userId,
    name: account.userName,
    avatar: 'src/assets/avatar/face-2.jpg'
};

const itemId = '123'; // 示例项目ID
</script>

<style scoped>
/* 添加你的样式 */
</style>