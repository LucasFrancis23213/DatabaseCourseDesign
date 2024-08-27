<template>
    <span class="text-title font-bold">输入查询条件以查询通知日志：<br></span>
    <div style="display: flex; align-items: center; margin-top:10px; gap:20px; margin-right:500px;">
      <a-input v-model:value="filters.userID" placeholder="用户ID" style="flex-grow: 1;">
          <template #prefix>
            <HourglassOutlined />
          </template>
      </a-input>
      <a-input v-model:value="filters.notificationType" placeholder="通知类型" style="flex-grow: 1;">
          <template #prefix>
            <HourglassOutlined />
          </template>
      </a-input>
      <a-date-picker v-model:value="filters.startDate" placeholder="开始日期" style="flex-grow: 1;" />
      <a-date-picker v-model:value="filters.endDate" placeholder="结束日期" style="flex-grow: 1;" />
      <a-select v-model:value="filters.status" placeholder="状态" style="flex-grow: 1;">
        <a-select-option value="已发送">已发送</a-select-option>
        <a-select-option value="未发送">未发送</a-select-option>
      </a-select>
      <div style="display: flex;">
        <a-button style="margin-right: 20px;" @click="applyFilters">
            查询
        </a-button>
      </div>
    </div>
    <br>
    <div class="table w-full">
      <a-table :columns="columns" :dataSource="data" rowKey="notificationID" />
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
    { title: '通知类型', dataIndex: 'Notification_Type' },
    { title: '发送日期', dataIndex: 'Send_Date' },
    { title: '状态', dataIndex: 'Status' },
  ];
  
  const filters = ref({
    userID: '',
    notificationType: '',
    startDate: '',
    endDate: '',
    status: '',
  });
  
  const data = ref([]);
  
  // 页面加载时获取所有通知日志
  onMounted(() => {
    applyFilters();
  });
  
  const applyFilters = async () => {
    try {
      const response = await axios.get('/api/LogsQuery/NotificationLogs', {
        params: {
          UserID: filters.value.userID || undefined,
          NotificationType: filters.value.notificationType || undefined,
          StartDate: filters.value.startDate || undefined,
          EndDate: filters.value.endDate || undefined,
          Status: filters.value.status || undefined,
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
    link.download = 'NotificationLogs.csv';
    link.click();
  
    URL.revokeObjectURL(url);
  };
  </script>
  