<template>
  <div class="add-ad-form">
    <h2 class="form-title">新增广告</h2>
    <form @submit.prevent="submitAd" class="ad-form">
      <div class="form-group">
        <label for="ad_content">广告内容</label>
        <input id="ad_content" v-model="adData.ad_content" placeholder="请输入广告内容" required>
      </div>
      <div class="form-group">
        <label for="ad_picture">广告图片URL</label>
        <input id="ad_picture" v-model="adData.ad_picture" placeholder="请输入图片URL" required>
      </div>
      <div class="form-group">
        <label for="ad_url">广告链接</label>
        <input id="ad_url" v-model="adData.ad_url" placeholder="请输入广告链接" required>
      </div>
      <div class="form-group">
        <label for="ad_type">广告类型</label>
        <select id="ad_type" v-model="adData.ad_type" required>
          <option value="promotion">促销</option>
          <option value="banner">横幅</option>
          <!-- 其他广告类型选项 -->
        </select>
      </div>
      <div class="form-group">
        <label for="start_time">开始时间</label>
        <input id="start_time" type="datetime-local" v-model="adData.start_time" required>
      </div>
      <div class="form-group">
        <label for="end_time">结束时间</label>
        <input id="end_time" type="datetime-local" v-model="adData.end_time" required>
      </div>
      <button type="submit" class="submit-btn">添加广告</button>
    </form>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import axios from "axios";
axios.defaults.baseURL = import.meta.env.VITE_API_URL_AD;

const emit = defineEmits(['adAdded']);

const adData = ref({
  ad_content: '',
  ad_picture: '',
  ad_url: '',
  ad_type: 'promotion',
  start_time: '',
  end_time: ''
});

const submitAd = async () => {
  try {
    const res = await axios.post('/api/advertisement/AddAdvertisement', adData.value);
    console.log(res);
    emit('adAdded');
    adData.value = {
      ad_content: '',
      ad_picture: '',
      ad_url: '',
      ad_type: 'promotion',
      start_time: '',
      end_time: ''
    };
    alert('广告添加成功！');
  } catch (error) {
    console.error('添加广告失败:', error);
    alert('添加广告失败，请重试。');
  }
};
</script>

<style scoped>
.add-ad-form {
  max-width: 500px;
  margin: 0 auto;
  padding: 20px;
  background-color: #f9f9f9;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.form-title {
  text-align: center;
  color: #333;
  margin-bottom: 20px;
}

.ad-form {
  display: flex;
  flex-direction: column;
}

.form-group {
  margin-bottom: 15px;
}

label {
  display: block;
  margin-bottom: 5px;
  color: #555;
  font-weight: bold;
}

input, select {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

input:focus, select:focus {
  outline: none;
  border-color: #4a90e2;
  box-shadow: 0 0 0 2px rgba(74, 144, 226, 0.2);
}

.submit-btn {
  background-color: #4a90e2;
  color: white;
  border: none;
  padding: 12px;
  border-radius: 4px;
  font-size: 16px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.submit-btn:hover {
  background-color: #3a7bc8;
}

@media (max-width: 600px) {
  .add-ad-form {
    padding: 15px;
  }
}
</style>