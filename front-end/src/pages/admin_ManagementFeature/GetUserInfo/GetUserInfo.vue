<template>
  <span class="text-title font-bold">输入用户ID或用户名以查询，直接查询获取全部用户：<br></span>
  <div style="display: flex; align-items: center; margin-top:10px; gap:20px; margin-right:500px;">
    <a-input v-model:value="filters.userID" placeholder="用户ID" style="flex-grow: 1;">
        <template #prefix>
          <HourglassOutlined />
        </template>
    </a-input>
    <a-input v-model:value="filters.userName" placeholder="用户名" style="flex-grow: 1;">
        <template #prefix>
          <HourglassOutlined />
        </template>
    </a-input>
    <div style="display: flex;">
  <a-button style="margin-right: 20px;" @click="applyFilters">
      查询
  </a-button>
</div>
</div>
<br>
<div class="table w-full">
  <a-table :columns="columns" :dataSource="data" rowKey="userID" />
</div>
<br>
<div style="display: flex; justify-content: flex-end;">
  <a-button type="primary" style="margin-right: 20px;" @click="downloadLogs">
      下载信息
  </a-button>
</div>

</template>
<script lang="ts" setup>
  import { ref } from 'vue';
  import { message } from 'ant-design-vue';
  import axios from 'axios';
  axios.defaults.baseURL = import.meta.env.VITE_API_URL;

const columns = [
    { title: '用户ID', dataIndex: 'UserID' },
    { title: '用户名', dataIndex: 'UserName' },
    { title: '联系方式', dataIndex: 'Contact' },
  ];

const filters = ref({
  userID: '',
  userName: ''
});

const data = ref([]);

const applyFilters = async () => {
  let query = '';
  if (filters.value.userID) {
    query += `UserID=${filters.value.userID}&`;
  }
  if (filters.value.userName) {
    query += `UserName=${filters.value.userName}&`;
  }
  if (query.endsWith('&')) {
    query = query.slice(0, -1);
  }
  try {
    const response = await axios.get(`/api/UserManagement/AdminGetUserInfo?${query}`);
    data.value = [response.data[0]];
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
}

const downloadLogs = () => {
  const now = new Date();
  const userID = data.value.length > 0 ? data.value[0].userID : 'UnknownUser';
  const headers = columns.map(col => col.title).join(',');
  const rows = data.value.map(row => {
    return columns.map(col => {
      if (col.dataIndex === 'contact') {
        return `=" ${row[col.dataIndex]}"`;
      }
      return row[col.dataIndex];
    }).join(',');
  });

  const csvContent = `\uFEFF${[headers, ...rows].join('\n')}`;
  const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
  const url = URL.createObjectURL(blob);

  const link = document.createElement('a');
  link.href = url;
  link.download = `UserInfo_userID_${userID}.csv`;
  link.click();

  URL.revokeObjectURL(url); 
}
</script>
