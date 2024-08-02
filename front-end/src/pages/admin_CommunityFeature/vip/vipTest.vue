<script setup lang="ts">
import vipAddItem from '@/components/CommunityFeature/vip/vipAddItem.vue'
import vipList from '@/components/CommunityFeature/vip/vipList.vue'
import vipDetails from '@/components/CommunityFeature/vip/vipDetails.vue'
import {ref} from "vue";

const user_id = ref(
  123,
);
</script>

<template>
  <vipAddItem></vipAddItem>
  <vipDetails :user_id='user_id'></vipDetails>
</template>

<style scoped lang="less">

</style>