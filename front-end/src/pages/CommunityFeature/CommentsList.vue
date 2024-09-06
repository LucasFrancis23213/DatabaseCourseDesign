<template>
    <div>
      <comments-list :item_id="itemId" :current_user="currentUser" />
    </div>
  </template>
  
  <script lang="ts" setup>
  import CommentsList from '../../components/CommunityFeature/CommentsList.vue';
  import { useAccountStore } from '@/store/account';
const {account,permissions} = useAccountStore();
// 示例用户数据
let currentUser = {
    id: account.userId,
    name: account.userName,
    avatar: account.avatar,
};
  
  const itemId = '123'; // 示例项目ID

  </script>
  
  <style scoped>
  /* 添加你的样式 */
  </style>