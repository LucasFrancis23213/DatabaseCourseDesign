<template>
  <div class="ad-details">
    <h2 class="ad-title">{{ ad.ad_content }}</h2>
    <div class="ad-info">
      <div class="ad-image">
        <img :src="ad.ad_picture" :alt="ad.ad_content"/>
      </div>
      <div class="ad-text">
        <p><strong>广告 ID:</strong> {{ ad.ad_id }}</p>
        <p><strong>类型:</strong> {{ getAdType(ad.ad_type) }}</p>
        <p><strong>开始时间:</strong> {{ formatDate(ad.start_time) }}</p>
        <p><strong>结束时间:</strong> {{ formatDate(ad.end_time) }}</p>
        <p><strong>显示次数:</strong> {{ entireAdInfo?.ad?.show_count }}</p>
        <p><strong>点击次数:</strong> {{ entireAdInfo?.ad?.click_count }}</p>


        <button @click="toggleClickDetails" class="details-button">
          {{ showClickDetails ? '隐藏点击详情' : '查看点击详情' }}
        </button>
        <div v-if="showClickDetails" class="click-details">
          <div class="table-container">
            <table class="styled-table">
              <thead>
              <tr>
                <th>用户ID</th>
                <th>点击时间</th>
                <th>IP地址</th>
              </tr>
              </thead>
              <tbody>
              <tr v-for="click in entireAdInfo.click_statistics" :key="click.user_id">
                <td>{{ click.user_id }}</td>
                <td>{{ formatDate(click.click_time) }}</td>
                <td>{{ click.ip_address }}</td>
              </tr>
              </tbody>
            </table>
          </div>
        </div>
        <a :href="ad.ad_url" target="_blank" class="ad-link">访问广告链接</a>
      </div>
    </div>
    <div class="ad-actions">
      <button @click="editAd" class="action-button edit">修改广告</button>
      <button @click="deleteAd" class="action-button delete">删除广告</button>
    </div>
    <div v-if="isEditing" class="edit-form">
      <div class="form-group">
        <label for="ad_content">广告内容:</label>
        <input id="ad_content" v-model="editedAd.ad_content" placeholder="请输入广告内容">
      </div>

      <div class="form-group">
        <label for="ad_url">广告链接:</label>
        <input id="ad_url" v-model="editedAd.ad_url" placeholder="请输入广告链接">
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
        <input id="ad_picture" v-model="editedAd.ad_picture" placeholder="请输入广告图片URL">
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
        <label for="ad_type">广告类型:</label>
        <input id="ad_type" v-model="editedAd.ad_type" placeholder="请选择广告类型">
      </div>

      <div class="form-group">
        <label for="start_time">开始时间:</label>
        <input id="start_time" type="datetime-local" v-model="editedAd.start_time" placeholder="请选择开始时间">
      </div>

      <div class="form-group">
        <label for="end_time">结束时间:</label>
        <input id="end_time" type="datetime-local" v-model="editedAd.end_time" placeholder="请选择结束时间">
      </div>

      <button @click="saveEdit" class="action-button save">保存修改</button>
      <button @click="cancelEdit" class="action-button cancel">取消</button>
    </div>
  </div>
</template>
<script setup>
import {onMounted, ref} from 'vue';
import axios from "axios";
import {message} from "ant-design-vue";

axios.defaults.baseURL = import.meta.env.VITE_API_URL;

const props = defineProps(['ad']);
const emit = defineEmits(['update', 'delete']);

const isEditing = ref(false);
const editedAd = ref({...props.ad});
const entireAdInfo = ref(0);
const showClickDetails = ref(false);

function formatDate(dateString) {
  const options = {year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit'};
  return new Date(dateString).toLocaleDateString('zh-CN', options);
}

function toggleClickDetails() {
  showClickDetails.value = !showClickDetails.value;
}

function getAdType(type) {
  const types = {
    'promotion': '促销',
    'event': '活动',
    'announcement': '公告'
    // 可以根据需要添加更多类型
  };
  return types[type] || type;
}

function editAd() {
  isEditing.value = true;
  editedAd.value = {...props.ad};
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
    editedAd.value.ad_picture = response.data.url;
    console.log('Upload successful:', editedAd.value.ad_picture);
  } catch (error) {
    console.error('Upload failed:', error);
  }
  return 1;
}
async function saveEdit() {
  try {
    if (imageInputType.value === "upload") {
      if (!await uploadAdImage()) {
        alert("请选择上传图片");
        return;
      }
    } else if (imageInputType.value === "") {
      alert("请选择上传图片或输入url");
      return;
    }
    if (!validateTime()) {
      message.error('广告结束时间必须晚于开始时间');
      return;
    }
    // 创建一个对象来存储被修改的参数
    const changedParams = {};

    // 比较原始广告数据和编辑后的数据
    for (const key in editedAd.value) {
      if (editedAd.value[key] !== props.ad[key]) {
        changedParams[key] = editedAd.value[key];
      }
    }
    // 如果没有修改任何参数，则不发送请求
    if (Object.keys(changedParams).length === 0) {
      alert('没有修改任何内容');
      isEditing.value = false;
      return;
    }

    // 发送修改请求到后台，只包含被修改的参数
    const response = await axios.put('/api/advertisement/UpdateAdvertisement', {
      ad_id: editedAd.value.ad_id,  // 总是包含广告ID
      ...changedParams
    });

    if (response.status === 200) {  // 假设 200 状态码表示成功
      // 更新成功
      Object.assign(props.ad, changedParams);  // 更新本地数据
      isEditing.value = false;

      alert('广告更新成功');
    } else {
      throw new Error('Failed to update ad');
    }
  } catch (error) {
    console.error('Error updating ad:', error);
    alert('更新广告失败，请稍后重试');
  }
}

function cancelEdit() {
  isEditing.value = false;
}
function validateTime() {
  return editedAd.value.end_time > editedAd.value.start_time;
}

async function deleteAd() {
  if (confirm('确定要删除这条广告吗？')) {
    try {
      // 发送删除请求到后台
      const response = await axios.delete('/api/advertisement/DeleteAdvertisement', {
        data: {
          ad_id: props.ad.ad_id
        }
      });

      if (response.status === 200) { // 假设 200 状态码表示成功
        // 删除成功
        alert('广告已成功删除');
        // 可能需要通知父组件广告已被删除
        emit('delete', props.ad.ad_id);
      } else {
        throw new Error('Failed to delete ad');
      }
    } catch (error) {
      console.error('Error deleting ad:', error);
      alert('删除广告失败，请稍后重试');
    }
  }
}

async function getEntireAdInfo() {
  try {
    const res = await axios.post('api/advertisement/GetAdvertisementDetails', {
      ad_id: editedAd.value.ad_id
    });
    entireAdInfo.value = res.data;

  } catch (e) {
    console.error(e);
    alert(`获取广告详情失败，错误原因：${e}`);
  }
}

onMounted(() => {
  getEntireAdInfo();
})

const imageInputType = ref('url');
const fileInput = ref(null);
const selectedFile = ref(null);
function toggleImageInput(type) {
  imageInputType.value = type;
  if (type === 'url') {
    selectedFile.value = null;
    if (fileInput.value) {
      fileInput.value.value = '';
    }
  } else {
    editedAd.ad_picture = props.ad.ad_picture;
  }
}
function handleFileChange(event) {
  selectedFile.value = event.target.files[0];
}
</script>

<style scoped>
.ad-details {
  width: 100%;
  background-color: #f9f9f9;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  margin-top: 10px;
  box-sizing: border-box;
}

.ad-title {
  color: #333;
  font-size: 1.5em;
  margin-bottom: 15px;
  border-bottom: 2px solid #4CAF50;
  padding-bottom: 10px;
}

.ad-info {
  display: flex;
  flex-wrap: wrap;
  gap: 20px;
}

.ad-image {
  flex: 1 1 200px;
  max-width: 100%;
}

.ad-text {
  flex: 1 1 300px;
}

.ad-image img {
  width: 100%;
  height: auto;
  border-radius: 4px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease;
}

.ad-image img:hover {
  transform: scale(1.05);
}

.ad-text p {
  margin: 10px 0;
  line-height: 1.6;
  color: #555;
}

.ad-text strong {
  color: #333;
}

.ad-link {
  display: inline-block;
  background-color: #4CAF50;
  color: white;
  padding: 10px 15px;
  text-decoration: none;
  border-radius: 4px;
  margin-top: 10px;
  transition: background-color 0.3s, transform 0.2s;
}

.ad-link:hover {
  background-color: #45a049;
  transform: translateY(-2px);
}

.details-button {
  background-color: #2196F3;
  color: white;
  border: none;
  padding: 8px 12px;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s;
  margin-bottom: 10px;
}

.details-button:hover {
  background-color: #1976D2;
}

.click-details h3 {
  color: #333;
  margin-bottom: 10px;
}

.table-container {
  overflow-x: auto;
}

.table-container {
  overflow-x: auto;
}

.styled-table {
  width: 100%;
  border-collapse: collapse;
  margin: 25px 0;
  font-size: 0.9em;
  font-family: sans-serif;
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.15);
}

.styled-table thead tr {
  background-color: #009879;
  color: #100505;
  text-align: left;
}

.styled-table th,
.styled-table td {
  padding: 12px 15px;
}

.styled-table tbody tr {
  border-bottom: 1px solid #dddddd;
}

.styled-table tbody tr:nth-of-type(even) {
  background-color: #f3f3f3;
}

.styled-table tbody tr:last-of-type {
  border-bottom: 2px solid #009879;
}

.styled-table tbody tr:hover {
  background-color: #f5f5f5;
  transition: background-color 0.3s ease;
}

table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 10px;
}

th, td {
  border: 1px solid #ddd;
  padding: 8px;
  text-align: left;
}

th {
  background-color: #f2f2f2;
  font-weight: bold;
}

tr:nth-child(even) {
  background-color: #f9f9f9;
}

.ad-actions {
  margin-top: 20px;
  display: flex;
  gap: 10px;
}

.action-button {
  padding: 10px 15px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s, transform 0.2s;
}

.action-button:hover {
  transform: translateY(-2px);
}

.edit {
  background-color: #FFC107;
  color: #000;
}

.delete {
  background-color: #F44336;
  color: white;
}

.edit-form {
  margin-top: 20px;
  background-color: #f5f5f5;
  padding: 20px;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.form-group {
  margin-bottom: 15px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  font-weight: bold;
  color: #333;
}

.form-group input {
  width: 100%;
  padding: 8px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  margin-top: 20px;
}

.action-button {
  padding: 10px 15px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  transition: background-color 0.3s, transform 0.2s;
  font-size: 14px;
}

.action-button:hover {
  transform: translateY(-2px);
}

.save {
  background-color: #4CAF50;
  color: white;
}

.cancel {
  background-color: #9E9E9E;
  color: white;
}

@media (max-width: 600px) {
  .ad-info {
    flex-direction: column;
  }

  .action-button {
    width: 100%;
  }
}
</style>