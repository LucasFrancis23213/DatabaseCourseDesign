<template>
  <span class="text-title font-bold">输入查询条件以查询推荐日志：<br></span>
  <div style="display: flex; flex-wrap: wrap; gap: 20px; margin-top:10px;">
      <div style="display: flex; flex-grow: 1; gap: 20px; width: 100%;">
          <a-input v-model:value="filters.userID" placeholder="用户ID" style="flex-grow: 1; min-width: 200px;">
              <template #prefix>
                  <HourglassOutlined />
              </template>
          </a-input>
          <a-input v-model:value="filters.recommendationType" placeholder="推荐类型" style="flex-grow: 1; min-width: 200px;">
              <template #prefix>
                  <HourglassOutlined />
              </template>
          </a-input>
          <a-date-picker v-model:value="filters.startTime" placeholder="开始时间" style="flex-grow: 1; min-width: 200px;" />
          <a-date-picker v-model:value="filters.endTime" placeholder="结束时间" style="flex-grow: 1; min-width: 200px;" />
          <a-input v-model:value="filters.userFeedback" placeholder="用户反馈" style="flex-grow: 1; min-width: 200px;">
              <template #prefix>
                  <HourglassOutlined />
              </template>
          </a-input>
      </div>
      <div style="display: flex; justify-content: flex-start; width: 100%; margin-top: 10px;">
          <a-button style="min-width: 100px;" @click="applyFilters">
              查询
          </a-button>
      </div>
  </div>
  <br>
  <div class="table w-full">
      <a-table :columns="columns" :dataSource="data" rowKey="logID" />
  </div>
  <br>
  <div style="display: flex; justify-content: flex-end;">
      <a-button type="primary" style="margin-right: 20px;" @click="downloadLogs">
          下载日志
      </a-button>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue';
import { message } from 'ant-design-vue';
import axios from 'axios';

axios.defaults.baseURL = import.meta.env.VITE_API_URL;

const columns = [
  { title: '用户ID', dataIndex: 'User_ID' },
  { title: '推荐类型', dataIndex: 'Recommendation_Type' },
  { title: '推荐时间', dataIndex: 'Recommendation_Time' },
  { title: '用户反馈', dataIndex: 'User_Feedback' },
];

const filters = ref({
  userID: '',
  recommendationType: '',
  startTime: '',
  endTime: '',
  userFeedback: '',
});

const data = ref([]);

// 页面加载时获取所有推荐日志
onMounted(() => {
  applyFilters();
});

const applyFilters = async () => {
  try {
      const response = await axios.get('/api/LogsQuery/RecommendationLogs', {
          params: {
              UserID: filters.value.userID || undefined,
              RecommendationType: filters.value.recommendationType || undefined,
              StartTime: filters.value.startTime || undefined,
              EndTime: filters.value.endTime || undefined,
              UserFeedback: filters.value.userFeedback || undefined,
          },
      });
      data.value = response.data;
  } catch (error) {
      let errorMessage = '查询失败: ';
      if (error.response && error.response.data) {
          errorMessage += error.response.data.message || JSON.stringify(error.response.data);
      } else {
          errorMessage += error.message;
      }
      message.error(errorMessage);
      console.error('Error fetching data:', error);
  }
};

const downloadLogs = () => {
  const headers = columns.map(col => col.title).join(',');
  const rows = data.value.map(row => columns.map(col => row[col.dataIndex]).join(',')).join('\n');
  
  const csvContent = `\uFEFF${[headers, rows].join('\n')}`;
  const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
  const url = URL.createObjectURL(blob);

  const link = document.createElement('a');
  link.href = url;
  link.download = 'RecommendationLogs.csv';
  link.click();

  URL.revokeObjectURL(url);
};
</script>
