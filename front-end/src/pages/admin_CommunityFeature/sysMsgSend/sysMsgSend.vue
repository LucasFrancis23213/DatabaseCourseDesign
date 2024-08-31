<script setup lang="ts">
import sendSystemMsg from "@/pages/CommunityFeature/chat/systemMsgSend";
import {ref} from "vue";

const userId = ref(301)
const sysMsgContent = ref("这是一个系统消息")

</script>

<template>
  <a-input
      v-model:value="userId"
      placeholder="想要发送系统消息的用户id"
  >
  </a-input>
  <a-input
      v-model:value="sysMsgContent"
      placeholder="想要发送的系统消息"
  >
  </a-input>
    <a-button @click="sendSystemMsg(userId,sysMsgContent,37)">发送</a-button>
  </template>


<style scoped lang="less">

</style>