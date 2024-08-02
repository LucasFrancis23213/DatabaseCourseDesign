<script setup lang="ts">
import vipAddItem from '@/components/CommunityFeature/vip/vipAddItem.vue'
import vipList from '@/components/CommunityFeature/vip/vipList.vue'
import vipDetails from '@/components/CommunityFeature/vip/vipDetails.vue'
import {ref} from "vue";

const vipInfo = ref({
  user_id:123,
  username:"ss",
  start_time:new Date().toLocaleDateString(),
  end_time:new Date()
})
</script>

<template>
  <vipAddItem></vipAddItem>
  <vipDetails :vip='vipInfo'></vipDetails>
</template>

<style scoped lang="less">

</style>