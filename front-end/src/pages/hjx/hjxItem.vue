<script setup lang="ts">
import ChatBubble from "@/components/chat/ChatBubble.vue";
import ChatScreen from "@/components/chat/ChatScreen.vue";
</script>

<template>
<div>
  <ChatBubble message="hello" :time="new Date().toLocaleTimeString()" avatar="./src/assets/logo/find.svg"></ChatBubble>
  <ChatScreen/>
</div>
</template>

<style scoped lang="less">

</style>