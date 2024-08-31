<template>
  <div class="add-ad-form">
    <h2 class="form-title">新增广告</h2>
    <form @submit.prevent="submitAd" class="ad-form">
      <div class="form-group">
        <label for="ad_content">广告内容</label>
        <input id="ad_content" v-model="adData.ad_content" placeholder="请输入广告内容" required>
      </div>
      <div class="form-group">
        <label>广告图片</label>
        <div class="toggle-buttons">
          <a-button
              @click="toggleImageInput('url')"
              :class="{ active: imageInputType === 'url' }"
          >
            输入URL
          </a-button>
          <a-button
              @click="toggleImageInput('upload')"
              :class="{ active: imageInputType === 'upload' }"
          >
            上传图片
          </a-button>
        </div>
      </div>
      <div v-if="imageInputType === 'url'" class="form-group">
        <label for="ad_picture">广告图片URL</label>
        <input id="ad_picture" v-model="adData.ad_picture" placeholder="请输入图片网络URL">
      </div>
      <div v-if="imageInputType === 'upload'" class="form-group">
        <label>广告图片上传</label>
        <a-input
            type="file"
            ref="fileInput"
            accept="image/*"
            placeholder="请选择图片"
            @change="handleFileChange"
        />
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
import {ref} from 'vue';
import axios from "axios";
import {message} from "ant-design-vue";

axios.defaults.baseURL = import.meta.env.VITE_API_URL;

const emit = defineEmits(['adAdded']);
const imageInputType = ref('');
const fileInput = ref(null);
const selectedFile = ref(null);
const adData = ref({
  ad_content: '',
  ad_picture: '',
  ad_url: '',
  ad_type: 'promotion',
  start_time: '',
  end_time: ''
});

function validateTime() {
  return adData.value.end_time > adData.value.start_time;
}

const submitAd = async () => {
  try {
    if (imageInputType.value === "upload") {
      if (!await uploadAdImage()) {
        message.error("请选择上传图片");
        return;
      }
    } else if (imageInputType.value === "") {
      message.error("请选择上传图片或输入url");
      return;
    }
    if (!validateTime()) {
      message.error('广告结束时间必须晚于开始时间');
      return;
    }
    console.log(adData.value);
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
    //alert('广告添加成功！');
    message.success('广告添加成功！')
  } catch (error) {
    message.error('添加广告失败，请重试。')
  }
};

function toggleImageInput(type) {
  imageInputType.value = type;
  if (type === 'url') {
    selectedFile.value = null;
    if (fileInput.value) {
      fileInput.value.value = '';
    }
  } else {
    adData.ad_picture = '';
  }
}

function handleFileChange(event) {
  selectedFile.value = event.target.files[0];
}

const uploadAdImage = async () => {
  if (!selectedFile.value) {
    return 0;
  }

  const formData = new FormData();
  formData.append('file', selectedFile.value);
  try {
    const response = await axios.post('/api/AdPicUpload/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
    adData.value.ad_picture = response.data.url;
    console.log('Upload successful:', adData.value.ad_picture);
  } catch (error) {
    console.error('Upload failed:', error);
  }
  return 1;
}
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