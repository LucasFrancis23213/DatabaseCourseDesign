<template>
  <div>
    <AdList v-if="!selectedAdId" @view-details="selectAd" />
    <AdDetails v-else :adId="selectedAdId" @back="selectedAdId = null" />
  </div>
</template>

<script setup>
import { ref } from 'vue';
import AdList from '@/components/CommunityFeature/advertisement/adList.vue';
import AdDetails from '@/components/CommunityFeature/advertisement/adDetailsItem.vue';
const selectedAdId = ref(null);

function selectAd(adId) {
  selectedAdId.value = adId;
}
</script>