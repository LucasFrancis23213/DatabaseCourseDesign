<template>
    <span class="text-title text-xl font-bold">筛选条件：<br></span>
    <div style="display: flex; align-items: center; margin-top:10px; gap:20px">
      <a-input v-model:value="filters.EventID" placeholder="事件ID" style="flex-grow: 1;">
          <template #prefix>
            <HourglassOutlined />
          </template>
      </a-input>
      <a-input v-model:value="filters.EventType" placeholder="事件类型" style="flex-grow: 1;">
          <template #prefix>
            <HourglassOutlined />
          </template>
      </a-input>
      <a-select v-model:value="filters.Status" placeholder="状态" style="flex-grow: 1; width:100%">
          <template #prefix>
            <HourglassOutlined />
          </template>
          <a-select-option value="">all</a-select-option>
          <a-select-option value="Open">Open</a-select-option>
          <a-select-option value="Closed">Closed</a-select-option>
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
      <a-table :columns="columns" :dataSource="sortedData" rowKey="Event_ID" />
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
  import { HourglassOutlined } from '@ant-design/icons-vue';
  
  const options = ref([
    { value: 'Open', label: 'Open' },
    { value: 'Closed', label: 'Closed' },
    { value: '', label: 'all' }
  ]);
  
  const columns = [
    { title: '事件ID', dataIndex: 'Event_ID' },
    { title: '事件类型', dataIndex: 'Event_Type' },
    { title: '状态', dataIndex: 'Status' },
    { title: '发生时间', dataIndex: 'Occurrence_Date' },
    { title: '事件详情', dataIndex: 'Event_Details' }
  ];
  
  const filters = ref({
    EventID: '',
    EventType: '',
    Status: '',
    StartTime: null,
    EndTime: null
  });
  
  const data = ref([]);
  
  const sortedData = computed(() => {
    return data.value.slice().sort((a, b) => a.Event_ID - b.Event_ID);
  });
  
  onMounted(async () => {
    try {
      const response = await axios.get('https://localhost:44343/api/GetSecurityEvents');
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
    if (filters.value.EventID) {
      query += `EventID=${filters.value.EventID}&`;
    }
    if (filters.value.EventType) {
      query += `EventType=${filters.value.EventType}&`;
    }
    if (filters.value.Status) {
      query += `Status=${filters.value.Status}&`;
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
      const response = await axios.get(`https://localhost:44343/api/GetSecurityEvents?${query}`);
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
    link.download = `SecurityEvents_${timestamp}.csv`;
    link.click();
  
    URL.revokeObjectURL(url); 
  }
  </script>
  