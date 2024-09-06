<script setup lang="ts">
import { ref, onMounted } from 'vue';
import AdItem from "@/components/CommunityFeature/advertisement/adItem.vue"
import axios from "axios";

axios.defaults.baseURL = import.meta.env.VITE_API_URL;
import {useAccountStore} from '@/store/account';

const {account, permissions} = useAccountStore();
const user_id = ref(account.userId);

interface Ad {
  id: number;
  url: string;
  content: string;
  picture: string;
  type:string;
}

const ad = ref<Ad | null>(null);

const getRandomAd = async () => {
  try {
    console.log("获取一次广告")
    const res = await axios.post("/api/advertisement/GetRandomAd",{user_id:user_id.value});
    ad.value = res.data.ad; // 假设响应数据中包含了广告信息
  } catch (e) {
    console.error(e);
  }
}

const isMember = async ()=>{
  try{
    const res =await axios.post("/api/vip/isMember",{
      user_id:user_id.value
    })
    return res.data.is_vip;
  }catch (e){
    console.error(e);
  }
}

onMounted(async() => {
  const memberStatus = await isMember();
  if( !memberStatus){
    getRandomAd();
  }

});
</script>

<template>
  <div class="ad-container">
    <AdItem v-if="ad"
      :adId="ad.id"
      :adUrl="ad.url"
      :adTitle="ad.content"
      :adImage="ad.picture"
    ></AdItem>
  </div>
</template>

<style scoped lang="less">
// 您的样式代码
</style>