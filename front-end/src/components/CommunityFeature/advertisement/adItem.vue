<template>
  <div class="ad-component" @click="navigateToAd">
    <img :src="props.adImage" :alt="props.adTitle" class="ad-image">
    <h3 class="ad-title">{{ props.adTitle }}</h3>
  </div>
</template>

<script setup lang="ts">
import { defineProps } from 'vue';

const props = defineProps({
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

const navigateToAd = () => {
  window.open(props.adUrl, '_blank');
};
</script>

<style scoped lang="less">
.ad-component {
  cursor: pointer;
  border: 1px solid #ddd;
  padding: 10px;
  max-width: 300px;
}

.ad-image {
  width: 100%;
  height: auto;
}

.ad-title {
  margin-top: 10px;
  font-size: 16px;
}
</style>