<template>
    <div>
      <comments-list :item_id="itemId" :current_user="currentUser" />
    </div>
  </template>
  
  <script lang="ts" setup>
  import CommentsList from '../../components/CommunityFeature/CommentsList.vue';
  
  const itemId = '123'; // 示例项目ID
  const currentUser = { id: 1, name: 'John Doe', avatar: 'src/assets/avatar/face-2.jpg' }; // 示例用户数据
  </script>
  
  <style scoped>
  /* 添加你的样式 */
  </style>