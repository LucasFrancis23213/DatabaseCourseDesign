<template>
  <span class="text-title text-xl font-bold">筛选条件：<br></span>
  <div style="display: flex; align-items: center; margin-top:10px; gap:30px">
    <a-input v-model:value="filters.SystemLogID" placeholder="系统日志ID" style="flex-grow: 1;">
        <template #prefix>
          <HourglassOutlined />
        </template>
    </a-input>
    <a-input v-model:value="filters.UserID" placeholder="用户ID" style="flex-grow: 1;">
        <template #prefix>
          <HourglassOutlined />
        </template>
    </a-input>
    <a-select v-model:value="filters.OperationType" placeholder="操作类型" style="flex-grow: 1; width:100%" >
        <template #prefix>
          <HourglassOutlined />
        </template>
        <a-select-option v-for="option in options" :value="option.value">
          {{ option.label }}
        </a-select-option>
    </a-select>
</div>
<br>
<div style="display: flex; justify-content: flex-end;">
  <a-button style="margin-right: 20px;" @click="applyFilters">
      筛选
  </a-button>
</div>
<br>
<div class="table w-full">
  <a-table :columns="columns" :dataSource="sortedData" rowKey="System_Log_ID" />
</div>
<br>
<div style="display: flex; justify-content: flex-end;">
  <a-button type="primary" style="margin-right: 20px;" @click="downloadLogs">
      下载日志
  </a-button>
</div>

</template>
<script lang="ts" setup>
  import { ref, onMounted, computed } from 'vue';
  import axios from 'axios';
  axios.defaults.baseURL = import.meta.env.VITE_API_URL;
  
  const options = ref([
  { value: '', label: '操作类型(all)' }
]);

const columns = [
    { title: '系统日志ID', dataIndex: 'System_Log_ID', width: 200 },
    { title: '操作类型', dataIndex: 'Operation_Type' },
    { title: '操作详情', dataIndex: 'Operation_Details', width: 500 },
    { title: '用户ID', dataIndex: 'User_ID' },
  ];

const filters = ref({
  SystemLogID: '',
  UserID: '',
  OperationType: '',
});

  const data = ref([]);

  const sortedData = computed(() => {
  return data.value.slice().sort((a, b) => a.System_Log_ID - b.System_Log_ID);
});

  onMounted(async () => {
  try {
    const response = await axios.get('/api/LogsQuery/SystemLogs');
    data.value = response.data;
  } catch (error) {
    console.error('Error fetching data:', error);
  }
});

function formatDateTime(date) {
  if (!date) return '';
  const d = new Date(date);
  const year = d.getFullYear();
  const month = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  const hours = String(d.getHours()).padStart(2, '0');
  const minutes = String(d.getMinutes()).padStart(2, '0');
  const seconds = String(d.getSeconds()).padStart(2, '0');
  return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`;
}

const applyFilters = async () => {
  let query = '';
  if (filters.value.SystemLogID) {
    query += `SystemLogID=${filters.value.SystemLogID}&`;
  }
  if (filters.value.UserID) {
    query += `UserID=${filters.value.UserID}&`;
  }
  if (filters.value.OperationType) {
    query += `OperationType=${filters.value.OperationType}&`;
  }
  if (query.endsWith('&')) {
    query = query.slice(0, -1);
  }
  try {
    const response = await axios.get(`/api/LogsQuery/SystemLogs?${query}`);
    data.value = response.data;
  } catch (error) {
    console.error('Error fetching data:', error);
  }
}

const downloadLogs = () => {
  const now = new Date();
  const timestamp = formatDateTime(now);
  const headers = columns.map(col => col.title).join(',');
  const rows = sortedData.value.map(row => {
    return columns.map(col => row[col.dataIndex]).join(',');
  });

  const csvContent = `\uFEFF${[headers, ...rows].join('\n')}`;
  const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
  const url = URL.createObjectURL(blob);

  const link = document.createElement('a');
  link.href = url;
  link.download = `SystemLog_${timestamp}.csv`;
  link.click();

  URL.revokeObjectURL(url); 
}
</script>
