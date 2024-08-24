<template>
  <div class="ad-component" @click="navigateToAd">
    <div class="ad-content">
      <img :src="props.adImage" :alt="props.adTitle" class="ad-image">
      <div class="ad-info">
        <h3 class="ad-title">{{ props.adTitle }}</h3>
        <p class="ad-description">{{ props.adDescription }}</p>
        <span class="ad-tag">广告</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { defineProps, ref } from 'vue';
import axios from "axios";

axios.defaults.baseURL = import.meta.env.VITE_API_URL;
import {useAccountStore} from '@/store/account';

const {account, permissions} = useAccountStore();
const user_id = ref(account.userId); //

const props = defineProps({
  adId: {
    type: Number,
    required: true
  },
  adImage: {
    type: String,
    required: true
  },
  adTitle: {
    type: String,
    required: true
  },
  adUrl: {
    type: String,
    required: true
  }
});

const navigateToAd = async () => {
  try {
    const res = await axios.post("/api/advertisement/ClickAd", {
      user_id: user_id.value,
      ad_id: props.adId,
      click_time: new Date().toISOString(),
    });
    console.log(res);
  } catch (e) {
    console.error(e);
  }
  window.open(props.adUrl, '_blank');
};
</script>

<style scoped lang="less">
.ad-component {
  cursor: pointer;
  border: 1px solid #e8e8e8;
  border-radius: 8px;
  overflow: hidden;
  background-color: #fff;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  transition: box-shadow 0.3s ease;
  margin-bottom: 16px;

  &:hover {
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  }
}

.ad-content {
  display: flex;
  padding: 12px;
}

.ad-image {
  width: 120px;
  height: 120px;
  object-fit: cover;
  border-radius: 4px;
}

.ad-info {
  flex: 1;
  margin-left: 12px;
  position: relative;
}

.ad-title {
  margin: 0 0 8px;
  font-size: 16px;
  font-weight: bold;
  color: #333;
}

.ad-description {
  font-size: 14px;
  color: #666;
  margin: 0;
  line-height: 1.4;
}

.ad-tag {
  position: absolute;
  top: 0;
  right: 0;
  background-color: #f0f0f0;
  color: #999;
  font-size: 12px;
  padding: 2px 6px;
  border-radius: 4px;
}
</style>