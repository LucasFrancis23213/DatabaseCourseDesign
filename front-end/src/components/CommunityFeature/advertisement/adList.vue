<template>
  <div class="ad-list-container">
    <h1 class="title">广告列表</h1>

    <div class="action-bar">
      <a-button @click="toggleAddNewAd()" class="btn btn-primary">
        {{ addNewAd ? '取消新增' : '新增广告' }}
      </a-button>
      <div class="search-container">
        <input
          v-model="searchQuery"
          @input="currentPage = 1"
          placeholder="搜索广告内容"
          class="search-input"
        >
      </div>
    </div>

    <transition name="fade">
      <AddAdForm v-if="addNewAd" @adAdded="refreshAdList" class="add-ad-form" />
    </transition>

    <ul class="ad-list">
      <li v-for="ad in displayedFilteredAds" :key="ad.ad_id" class="ad-item">
        <div class="ad-content">{{ ad.ad_content }}</div>
        <a-button @click="toggleDetails(ad)" class="btn btn-secondary">
          {{ ad.showDetails ? '关闭详情' : '查看详情' }}
        </a-button>
        <transition name="expand">
          <AdDetailsItem v-if="ad.showDetails" :ad="ad" @close="toggleDetails(ad)" />
        </transition>
      </li>
    </ul>

    <div class="pagination">
      <a-button @click="prevPage" :disabled="currentPage === 1" class="btn btn-outline">上一页</a-button>
      <span class="page-info">{{ currentPage }} / {{ totalFilteredPages }}</span>
      <a-button @click="nextPage" :disabled="currentPage === totalFilteredPages" class="btn btn-outline">下一页</a-button>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import axios from "axios";
import AdDetailsItem from './AdDetailsItem.vue';  // 确保导入 AdDetailsItem 组件
import AddAdForm from './adAddItem.vue';

axios.defaults.baseURL = import.meta.env.VITE_API_URL;

const ads = ref([]);
const currentPage = ref(1);
const pageSize = 10;
const searchQuery = ref('');


const refreshAdList = async () => {
  await getAds(); // 重新获取广告列表
};

const filteredAds = computed(() => {
  if (!searchQuery.value) return ads.value;
  return ads.value.filter(ad =>
    ad.ad_content.toLowerCase().includes(searchQuery.value.toLowerCase())
  );
});

const totalFilteredPages = computed(() => Math.ceil(filteredAds.value.length / pageSize));

const displayedFilteredAds = computed(() => {
  const start = (currentPage.value - 1) * pageSize;
  const end = start + pageSize;
  return filteredAds.value.slice(start, end);
});

function prevPage() {
  if (currentPage.value > 1) currentPage.value--;
}

function nextPage() {
  if (currentPage.value < totalFilteredPages.value) currentPage.value++;
}


function toggleDetails(ad) {
  ad.showDetails = !ad.showDetails;
}

const addNewAd = ref(false);
function toggleAddNewAd(){
  addNewAd.value=!addNewAd.value;
}


async function getAds() {
  try {
    const res = await axios.post('/api/advertisement/GetAdvertisements');
    console.log(res);
    ads.value = res.data.advertisements.map(ad => ({...ad, showDetails: false}));
  } catch (e) {
    alert(`获取广告列表失败，错误原因：${e}`);
    console.error(e);
  }
}

onMounted(async () => {
  await getAds();
});
</script>

<style scoped>
:root {
  --primary-color: #438bbb;
  --primary-dark: #2980b9;
  --secondary-color: #2ecc71;
  --secondary-dark: #27ae60;
  --background-color: #f5f7fa;
  --text-color: #333;
  --border-color: #e0e0e0;
}

.ad-list-container {
  max-width: 800px;
  margin: 0 auto;
  padding: 20px;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
  background-color: var(--background-color);
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.title {
  color: var(--text-color);
  text-align: center;
  margin-bottom: 30px;
  font-size: 2.5rem;
  font-weight: 300;
}

.action-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}



.search-container {
  flex-grow: 1;
  margin-left: 20px;
}

.search-input {
  width: 100%;
  padding: 10px;
  font-size: 1rem;
  border: 1px solid var(--border-color);
  border-radius: 4px;
  transition: border-color 0.3s ease;
}

.search-input:focus {
  outline: none;
  border-color: var(--primary-color);
}

.ad-list {
  list-style-type: none;
  padding: 0;
}

.ad-item {
  display: flex;
  flex-wrap: wrap;
  justify-content: space-between;
  align-items: center;
  padding: 15px;
  border-bottom: 1px solid var(--border-color);
  transition: background-color 0.3s ease;
}

.ad-item:hover {
  background-color: rgba(52, 152, 219, 0.1);
}

.ad-content {
  flex-grow: 1;
  margin-right: 20px;
}

.ad-item .btn {
  margin-left: 10px;
}

.pagination {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-top: 30px;
}

.page-info {
  margin: 0 15px;
  font-size: 1rem;
}

.add-ad-form {
  margin-bottom: 20px;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.5s ease;
}

.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

.expand-enter-active,
.expand-leave-active {
  transition: all 0.3s ease-out;
  max-height: 1000px;
  overflow: hidden;
}

.expand-enter-from,
.expand-leave-to {
  max-height: 0;
  opacity: 0;
}

@media (max-width: 600px) {
  .action-bar {
    flex-direction: column;
    align-items: stretch;
  }

  .search-container {
    margin-left: 0;
    margin-top: 10px;
  }

  .ad-item {
    flex-direction: column;
    align-items: flex-start;
  }
.ad-item .btn {
    margin-left: 0;
    margin-top: 10px;
  }
  .btn {
    width: 100%;
    margin-top: 10px;
  }
}
</style>