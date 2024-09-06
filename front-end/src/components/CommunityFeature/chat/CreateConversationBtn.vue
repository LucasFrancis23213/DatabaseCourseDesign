<template>
  <a-button @click="openConversationInput">
    {{ buttonText }}
  </a-button>

</template>

<script setup>

import { useAccountStore } from '@/store/account';
const {account, permissions} = useAccountStore();

import {useRouter} from "vue-router";
const router = useRouter();
const props = defineProps({
  targetId: {
    type: Number,
    required: true
  },
  buttonText: {
    type: String,
    default: '进入会话'
  }
});

const openConversationInput = () => {
  router.push({
    name: "聊天",
    params: {
      "conversation_id": props.targetId,
    },
    query: { "current_user_id":account.userId, }
  })
};

</script>

