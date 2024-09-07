<template>
  <span class="text-title text-xl font-bold">筛选条件：<br></span>
  <div style="display: flex; align-items: center; margin-top:10px; gap:10px">
    <a-input v-model:value="filters.FromUserID" placeholder="FromID" style="flex-grow: 1;">
        <template #prefix>
          <HourglassOutlined />
        </template>
    </a-input>
    <a-input v-model:value="filters.ToUserID" placeholder="ToID" style="flex-grow: 1;">
        <template #prefix>
          <HourglassOutlined />
        </template>
    </a-input>
    <a-input v-model:value="filters.ItemID" placeholder="物品ID" style="flex-grow: 1;">
        <template #prefix>
          <HourglassOutlined />
        </template>
    </a-input>
    <a-input-number
      v-model:value="filters.MinAmount"
      placeholder="最低金额"
      :min="0.01"
      :step="0.01"
      style="flex-grow: 1; width:100%;"
      :precision="2"
    />
    <a-input-number
      v-model:value="filters.MaxAmount"
      placeholder="最高金额"
      :min="filters.MinAmount === null ? 0.01 : filters.MinAmount"
      :step="0.01"
      style="flex-grow: 1; width:100%;"
      :precision="2"
    />
    <a-select v-model:value="filters.TransactionType" placeholder="操作类型" style="flex-grow: 1; width:100%" >
        <template #prefix>
          <HourglassOutlined />
        </template>
        <a-select-option v-for="option in options" :value="option.value">
          {{ option.label }}
        </a-select-option>
    </a-select>
    <a-input v-model:value="filters.TransactionID" placeholder="交易ID" style="flex-grow: 1;">
        <template #prefix>
          <HourglassOutlined />
        </template>
    </a-input>
    <a-input v-model:value="filters.Status" placeholder="状态" style="flex-grow: 1;">
        <template #prefix>
          <HourglassOutlined />
        </template>
    </a-input>
</div>
<div style="display: flex; align-items: center; margin-top:20px; gap:10px">
  <span class="text-subtext font-semibold">开始交易范围:</span>
  <a-date-picker
      v-model:value="filters.StartTimeBeg"
      placeholder="开始时间"
      :show-time="{ format: 'HH:mm:ss' }"
      format="YYYY-MM-DDTHH:mm:ss"
      style="flex-grow: 1;"
    />
    <a-date-picker
      v-model:value="filters.StartTimeEnd"
      placeholder="结束时间"
      :show-time="{ format: 'HH:mm:ss' }"
      format="YYYY-MM-DDTHH:mm:ss"
      style="flex-grow: 1;"
    />
    <span class="text-subtext font-semibold" style="margin-left:10px">结束交易范围:</span>
    <a-date-picker
      v-model:value="filters.FinishTimeBeg"
      placeholder="开始时间"
      :show-time="{ format: 'HH:mm:ss' }"
      format="YYYY-MM-DDTHH:mm:ss"
      style="flex-grow: 1;"
    />
    <a-date-picker
      v-model:value="filters.FinishTimeEnd"
      placeholder="结束时间"
      :show-time="{ format: 'HH:mm:ss' }"
      format="YYYY-MM-DDTHH:mm:ss"
      style="flex-grow: 1;"
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
    <a-table :columns="columns" :dataSource="sortedData" rowKey="Transaction_ID">
      <template #bodyCell="{ column, record }">
        <template v-if="column.dataIndex === 'Operation'">
          <a-button type="link" @click="edit(record)">
          <template #icon>
            <EditFilled />
          </template>
          更新
        </a-button>
        </template>
        <template v-else>
          {{ record[column.dataIndex] }}
        </template>
      </template>
    </a-table>
  </div>
<br>
<div style="display: flex; justify-content: flex-end;">
  <a-button type="primary" style="margin-right: 20px;" @click="downloadLogs">
      下载日志
  </a-button>
</div>
<a-modal
    v-model:visible="isModalVisible"
    title="更新交易记录"
    ok-text="确认"
    cancel-text="取消"
    @ok="updateChanges"
    @cancel="cancelEdit"
  >
  <a-form :model="editRecord" :labelCol="{ span: 5 }" :wrapperCol="{ span: 15 }">
    <a-form-item label="交易ID" required name="TransactionID">
      <span>{{ editRecord.TransactionID }}</span>
    </a-form-item>
    <a-form-item label="更新状态" required name="newStatus">
      <a-input v-model:value="editRecord.newStatus"  />
    </a-form-item>
    <a-form-item label="结束时间" name="FinishTime">
    <div>
      <a-date-picker
        v-model:value="editRecord.FinishTime"
        placeholder="结束时间"
        :show-time="{ format: 'HH:mm:ss' }"
        format="YYYY-MM-DDTHH:mm:ss"
        style="width: 100%;"
    />
    </div>
    </a-form-item>
  </a-form>
  </a-modal>
</template>
<script lang="ts" setup>
  import { ref, onMounted, computed } from 'vue';
  import { EditFilled } from '@ant-design/icons-vue';
  import { message } from 'ant-design-vue';
  import axios from 'axios';
  
  axios.defaults.baseURL = import.meta.env.VITE_API_URL;
  
  const options = ref([
  { value: '', label: '交易类型(all)' }
]);

const columns = [
  { title: '交易ID', dataIndex: 'Transaction_ID' },
  { title: 'FromUserID', dataIndex: 'From_User_ID' },
  { title: 'ToUserID', dataIndex: 'To_User_ID' },
  { title: '物品ID', dataIndex: 'Item_ID' },
  { title: '交易金额(元)', dataIndex: 'Amount' },
  { title: '交易类型', dataIndex: 'Transaction_Type' },
  { title: '状态', dataIndex: 'Status' },
  { title: '开始交易时间', dataIndex: 'StartTime' },
  { title: '结束交易时间', dataIndex: 'FinishTime' },
  { title: '操作', dataIndex: 'Operation' },
];

const filters = ref({
  FromUserID: '',
  ToUserID: '',
  ItemID: '',
  MinAmount:'',
  MaxAmount:'',
  TransactionType:'',
  TransactionID:'',
  Status:'',
  StartTimeBeg: null,
  StartTimeEnd: null,
  FinishTimeBeg: null,
  FinishTimeEnd: null,
});

const data = ref([]);

const editRecord = ref({
  TransactionID: '',
  newStatus: '',
  FinishTime: null,
});

const isModalVisible = ref(false);

function edit(row) {
  editRecord.value = {
    TransactionID: row.Transaction_ID,
    newStatus: '',
    FinishTime: null,
  };
  isModalVisible.value = true;
}

function updateChanges() {
  const { TransactionID, newStatus, FinishTime } = editRecord.value;

  if (!newStatus) {
    message.error('“更新状态”不能为空');
    return; 
  }

  let url = `http://121.36.200.128:5001/api/TransactionLogs/UpdateTransactionStatus?TransactionID=${TransactionID}&newStatus=${newStatus}`;

  if (FinishTime) {
    url += `&FinishTime=${formatDateTime(FinishTime)}`;
  }

  axios.put(url)
    .then(() => {
      message.success('记录已更新');
      applyFilters();
      isModalVisible.value = false;
    })
    .catch(error => {
      message.error('更新失败，请重试');
      console.error('Error updating transaction:', error);
    });
}

function cancelEdit() {
  editRecord.value = {
    TransactionID: '',
    newStatus: '',
    FinishTime: null,
  };
  isModalVisible.value = false;
}

const sortedData = computed(() => {
  return data.value.slice().sort((a, b) => a.Transaction_ID - b.Transaction_ID);
});

onMounted(async () => {
  try {
    const response = await axios.get('/api/TransactionLogs/GetTransactionLogs');
    data.value = response.data.map(item => ({
      ...item,
      Operation: 'edit', 
    }));
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
  if (filters.value.FromUserID) {
    query += `FromUserID=${filters.value.FromUserID}&`;
  }
  if (filters.value.ToUserID) {
    query += `ToUserID=${filters.value.ToUserID}&`;
  }
  if (filters.value.ItemID) {
    query += `ItemID=${filters.value.ItemID}&`;
  }
  if (filters.value.MinAmount) {
    query += `MinAmount=${filters.value.MinAmount}&`;
  }
  if (filters.value.MaxAmount) {
    query += `MaxAmount=${filters.value.MaxAmount}&`;
  }
  if (filters.value.TransactionType) {
    query += `TransactionType=${filters.value.TransactionType}&`;
  }
  if (filters.value.TransactionID) {
    query += `TransactionID=${filters.value.TransactionID}&`;
  }
  if (filters.value.Status) {
    query += `Status=${filters.value.Status}&`;
  }
  if (filters.value.StartTimeBeg) {
    query += `StartTimeBeg=${formatDateTime(filters.value.StartTimeBeg)}&`;
  }
  if (filters.value.StartTimeEnd) {
    query += `StartTimeEnd=${formatDateTime(filters.value.StartTimeEnd)}&`;
  }
  if (filters.value.FinishTimeBeg) {
    query += `FinishTimeBeg=${formatDateTime(filters.value.FinishTimeBeg)}&`;
  }
  if (filters.value.FinishTimeEnd) {
    query += `FinishTimeEnd=${formatDateTime(filters.value.FinishTimeEnd)}&`;
  }
  if (query.endsWith('&')) {
    query = query.slice(0, -1);
  }
  try {
    const response = await axios.get(`/api/TransactionLogs/GetTransactionLogs?${query}`);
    data.value = response.data.map(item => ({
      ...item,
      operation: 'edit', 
    }));
  } catch (error) {
    console.error('Error fetching data:', error);
  }
}

const downloadLogs = () => {
  const now = new Date();
  const timestamp = formatDateTime(now);
  const headers = columns.slice(0, -1).map(col => col.title).join(',');
  const rows = sortedData.value.map(row => {
    return columns.slice(0, -1).map(col => row[col.dataIndex]).join(',');
  });

  const csvContent = `\uFEFF${[headers, ...rows].join('\n')}`;
  const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
  const url = URL.createObjectURL(blob);

  const link = document.createElement('a');
  link.href = url;
  link.download = `TransactionLog_${timestamp}.csv`;
  link.click();

  URL.revokeObjectURL(url); 
}
</script>
