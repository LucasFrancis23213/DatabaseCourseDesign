<template>
    <a-card title="Answers List" class="conversations rounded-xl shadow-lg" :bordered="false">
    <div class="chat flex items-center" v-for="(Answer,index) in AnswersList">
      <img class="w-12 rounded-xl" :src="Answer.user_avatar" />
      <div class="content ml-3 flex-1">
        <div class="name">{{ Answer.user_name }}</div>
        <div class="message text-sm text-subtext">{{ Answer.content }}</div>
      </div>
      <!--<div class="action">
        <a-button class="text-sm font-semibold" type="link" @click = 'Questionclick'>回答详细</a-button>
      </div>-->
    </div>
  </a-card>
  </template>
  
  <script lang="ts" setup>
  import { useRoute } from 'vue-router';
  
  // 获取路由参数
  const route = useRoute();
  const AnswersList = JSON.parse(route.query.AnsList as string); // 反序列化对象
  </script>
  
  <style scoped>
  /* 样式 */
  </style>