<script setup lang="ts">
import axios from 'axios';
import { useRoute } from 'vue-router';
import {message} from "ant-design-vue";

axios.defaults.baseURL = import.meta.env.VITE_API_URL;
const route = useRoute();
async function confirmRecharge(){
  try{
    console.log(route.params.recharge_id)
    const res = await axios.post('api/recharge/confirm',{
      recharge_id:route.params.recharge_id
    })
    console.log(res);
  }catch (e){
    message.error(e);
    console.error(e);
  }
}
</script>

<template>
<a-button @click="confirmRecharge">确认</a-button>
</template>

<style scoped lang="less">

</style>