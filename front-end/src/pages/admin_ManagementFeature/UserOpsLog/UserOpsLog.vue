<template>
  <span class="text-title text-xl font-bold">筛选条件：<br></span>
  <div style="display: flex; align-items: center; margin-top:10px; gap:20px">
    <a-input v-model:value="filters.ActivityLogID" placeholder="操作ID" style="flex-grow: 1;">
        <template #prefix>
          <HourglassOutlined />
        </template>
    </a-input>
    <a-input v-model:value="filters.UserID" placeholder="用户ID" style="flex-grow: 1;">
        <template #prefix>
          <HourglassOutlined />
        </template>
    </a-input>
    <a-select v-model:value="filters.ActionType" placeholder="操作类型" style="flex-grow: 1; width:100%" >
        <template #prefix>
          <HourglassOutlined />
        </template>
        <a-select-option v-for="option in options" :value="option.value">
          {{ option.label }}
        </a-select-option>
    </a-select>
    <a-date-picker
      v-model:value="filters.StartTime"
      placeholder="开始时间"
      :show-time="{ format: 'HH:mm:ss' }"
      format="YYYY-MM-DDTHH:mm:ss"
      style="flex-grow: 1; width:100%;"
    />
    <a-date-picker
      v-model:value="filters.EndTime"
      placeholder="结束时间"
      :show-time="{ format: 'HH:mm:ss' }"
      format="YYYY-MM-DDTHH:mm:ss"
      style="flex-grow: 1; width:100%;"
    />
</div>
<br>
<div style="display: flex; justify-content: flex-end;">
  <a-button style="margin-right: 20px;" @click="applyFilters">
      筛选
  </a-button>
</div>
<br>
<div class="table w-full">
  <a-table :columns="columns" :dataSource="sortedData" rowKey="Activity_Log_ID" />
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
  
  const options = ref([
  { value: 'Login', label: 'Login' },
  { value: 'Logout', label: 'Logout' },
  { value: 'Signup', label: 'Signup' },
  { value: 'DeleteUser', label: 'DeleteUser' },
  { value: '', label: '操作类型(all)' }
]);

const columns = [
    { title: '操作ID', dataIndex: 'Activity_Log_ID' },
    { title: '用户ID', dataIndex: 'User_ID' },
    { title: '操作类型', dataIndex: 'Action_Type' },
    { title: '操作时间', dataIndex: 'Occurrence_Time' },
  ];

const filters = ref({
  ActivityLogID: '',
  UserID: '',
  ActionType: '',
  StartTime: null,
  EndTime: null,
});

  const data = ref([]);

  const sortedData = computed(() => {
  return data.value.slice().sort((a, b) => a.Activity_Log_ID - b.Activity_Log_ID);
});

  onMounted(async () => {
  try {
    const response = await axios.get('https://localhost:44343/api/GetUserOpsLogs');
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
  if (filters.value.ActivityLogID) {
    query += `ActivityLogID=${filters.value.ActivityLogID}&`;
  }
  if (filters.value.UserID) {
    query += `UserID=${filters.value.UserID}&`;
  }
  if (filters.value.ActionType) {
    query += `ActionType=${filters.value.ActionType}&`;
  }
  if (filters.value.StartTime) {
    query += `StartTime=${formatDateTime(filters.value.StartTime)}&`;
  }
  if (filters.value.EndTime) {
    query += `EndTime=${formatDateTime(filters.value.EndTime)}&`;
  }
  if (query.endsWith('&')) {
    query = query.slice(0, -1);
  }
  try {
    const response = await axios.get(`https://localhost:44343/api/GetUserOpsLogs?${query}`);
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
  link.download = `UserOpsLog_${timestamp}.csv`;
  link.click();

  URL.revokeObjectURL(url); 
}
</script>
