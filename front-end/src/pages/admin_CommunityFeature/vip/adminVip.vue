<script setup lang="ts">
import vipAddItem from '@/components/CommunityFeature/vip/vipAddItem.vue'
import vipDetails from '@/components/CommunityFeature/vip/vipDetails.vue'
import {ref} from "vue";

import {useAccountStore} from '@/store/account';
const {account, permissions} = useAccountStore();
const user_id = ref(account.userId);
</script>

<template>
  <vipAddItem :user_id="user_id"></vipAddItem>
  <vipDetails :user_id='user_id'></vipDetails>
</template>

<style scoped lang="less">

</style>