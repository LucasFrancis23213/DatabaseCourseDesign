<template>
  <div class="ad-list-container">
    <h1 class="title">广告列表</h1>
    <div class="action-bar">
      <a-button class="editable-add-btn" type="primary" @click="toggleAddNewAd">
        {{ addNewAd ? '取消新增' : '新增广告' }}
      </a-button>
      <div class="search-container">
        <a-input
            v-model:value="searchQuery"
            @input="currentPage = 1"
            placeholder="搜索广告内容"
            class="search-input"
        />
      </div>
    </div>
    <a-table :columns="columns" :data-source="displayedFilteredAds" :pagination="{ pageSize: 10 }" @expand="onExpand" :expandedRowKeys="expandedRowKeys"
  row-key="ad_id">
      <template #expandedRowRender="{record}">
        <a-table
            v-if="record.showDetails"
            :columns="statisticsColumns"
            :data-source="[{ click_count: record.click_count, show_count: record.show_count }]"
            :loading="record.loading"
            :pagination="false"
        >
          <template #bodyCell="{ text, record, index, column}">
          </template>
        </a-table>
        <a-table
            v-if="record.showDetails"
            :columns="innerColumns"
            :data-source="record.click_statistics"
            :loading="record.loading"
            :pagination="false"
        >
          <template #bodyCell="{ text, record, index, column}">
          </template>
        </a-table>

      </template>
      <template #bodyCell="{ column, record }">

        <template v-if="column.dataIndex === 'ad_content'">
          <div>
            <a-input
                v-if="editingKey === record.ad_id"
                v-model:value="editableData[record.ad_id][column.dataIndex]"
                style="margin: -5px 0"
            />
            <template v-else>
              {{ record[column.dataIndex] }}
            </template>
          </div>
        </template>
        <template v-else-if="column.dataIndex === 'ad_type'">
          <div>
            <a-select
                v-if="editingKey === record.ad_id"
                v-model:value="editableData[record.ad_id][column.dataIndex]"
                style="width: 100%"
            >
              <a-select-option value="promotion">促销广告</a-select-option>
              <a-select-option value="banner">横幅广告</a-select-option>
              <a-select-option value="popup">弹窗广告</a-select-option>
              <a-select-option value="sidebar">侧边栏广告</a-select-option>
            </a-select>
            <template v-else>
              {{ AD_TYPE[record[column.dataIndex]] }}
            </template>
          </div>
        </template>
        <template v-else-if="column.dataIndex === 'start_time' || column.dataIndex === 'end_time'">
          <div>
            <a-date-picker
                v-if="editingKey === record.ad_id"
                v-model:value="editableData[record.ad_id][column.dataIndex]"
                style="width: 100%"
                show-time
                format="YYYY-MM-DD HH:mm:ss"
                :value-format="dateValueFormat"
            />

            <template v-else>
              {{ formatDate(record[column.dataIndex]) }}
            </template>
          </div>
        </template>
        <template v-else-if="column.dataIndex === 'ad_picture'">
          <div>
            <template v-if="editingKey === record.ad_id">
              <a-radio-group v-model:value="editableData[record.ad_id].imageInputType">
                <a-radio-button value="url">URL</a-radio-button>
                <a-radio-button value="upload">上传</a-radio-button>
              </a-radio-group>
              <a-input
                  v-if="editableData[record.ad_id].imageInputType === 'url'"
                  v-model:value="editableData[record.ad_id][column.dataIndex]"
                  placeholder="输入图片URL"
                  style="margin-top: 8px"
              />
              <a-upload
                  v-else
                  :before-upload="beforeUpload"
                  @change="handleImageChange"
              >
                <a-button>上传图片</a-button>
              </a-upload>
            </template>
            <a-image v-else :src="record[column.dataIndex]" :width="100"/>
          </div>
        </template>
        <template v-else-if="column.dataIndex === 'ad_url'">
          <div>
            <a-input
                v-if="editingKey === record.ad_id"
                v-model:value="editableData[record.ad_id][column.dataIndex]"
                style="margin: -5px 0"
            />
            <a v-else :href="record[column.dataIndex]" target="_blank">{{ record[column.dataIndex] }}</a>
          </div>
        </template>
        <template v-else-if="column.dataIndex === 'action'">
          <span v-if="editingKey === record.ad_id">
            <a-typography-link @click="save(record.ad_id)">保存</a-typography-link>
            <a-typography-link @click="cancel(record.ad_id)" style="margin-left: 8px">取消</a-typography-link>
          </span>
          <span v-else>
            <a-typography-link @click="edit(record.ad_id)">编辑</a-typography-link>
          </span>

          <a-popconfirm
              title="确定要删除这条广告吗？"
              @confirm="() => deleteAd(record.ad_id)"
          >
            <a-typography-link style="margin-left: 8px">删除</a-typography-link>
          </a-popconfirm>
        </template>
      </template>
    </a-table>
  </div>
</template>

<script setup>
import {ref, computed, onMounted, reactive} from 'vue';
import axios from 'axios';
import {message} from 'ant-design-vue';
import AdDetailsItem from './AdDetailsItem.vue';
import dayjs from "dayjs";

axios.defaults.baseURL = import.meta.env.VITE_API_URL;

const ads = ref([]);
const currentPage = ref(1);
const pageSize = ref(10);
const searchQuery = ref('');
const addNewAd = ref(false);
const selectedAd = ref(null);
const editingKey = ref('');
const editableData = reactive({});

const columns = [
  {title: 'ID', dataIndex: 'ad_id', key: 'ad_id', fixed: 'left', width: '5%', ellipsis: true},
  {title: '广告内容', dataIndex: 'ad_content', key: 'ad_content'},
  {title: '类型', dataIndex: 'ad_type', key: 'ad_type', width: '10%'},
  {title: '开始时间', dataIndex: 'start_time', key: 'start_time'},
  {title: '结束时间', dataIndex: 'end_time', key: 'end_time'},
  {title: '图片', dataIndex: 'ad_picture', key: 'ad_picture'},
  {title: '链接', dataIndex: 'ad_url', key: 'ad_url'},
  {title: '操作', dataIndex: 'action', key: 'action', fixed: 'right'},
];
const innerColumns = [
  {title: '用户ID', dataIndex: 'user_id', key: 'user_id'},
  {title: '点击时间', dataIndex: 'click_time', key: 'click_time'},
  {title: 'IP地址', dataIndex: 'ip_address', key: 'ip_address'},
];
const statisticsColumns = [
  {title: '展示次数', dataIndex: 'show_count', key: 'show_count'},
  {title: '点击次数', dataIndex: 'click_count', key: 'click_count'},
];
const AD_TYPE = {
  "promotion": '促销广告',
  "banner": "横幅广告",
  "popup": "弹窗广告",
  "sidebar": "侧边栏广告"
}

const dateValueFormat = 'YYYY-MM-DDTHH:mm:ss';

const formatDate = (dateString) => {
  return dayjs(dateString).format(dateValueFormat);
};

function handleAdd() {
  addNewAd.value = true;
  const newAd = {
    ad_id: `new-${Date.now()}`, // 临时ID，后端会分配真正的ID
    ad_content: '新广告内容',
    ad_type: 'banner',
    start_time: dayjs(),
    end_time: dayjs().add(7, 'day'),
    ad_picture: '',
    ad_url: '',
    showDetails: false
  };

  ads.value = [newAd, ...ads.value];
  edit(newAd.ad_id); // 立即进入编辑模式
}

const deleteAd = async (ad_id) => {
  // if (confirm('确定要删除这条广告吗？')) {
  try {
    const response = await axios.delete('/api/advertisement/DeleteAdvertisement', {
      data: {ad_id}
    });

    if (response.status === 200) {
      message.success('广告已成功删除');
      await refreshAdList();
    } else {
      throw new Error('Failed to delete ad');
    }
  } catch (error) {
    console.error('Error deleting ad:', error);
    message.error('删除广告失败，请稍后重试');
  }
  // }
};

const refreshAdList = async () => {
  await getAds();
  message.success('广告列表已更新');
};

const filteredAds = computed(() => {
  if (!searchQuery.value) return ads.value;
  return ads.value.filter(ad =>
      ad.ad_content.toLowerCase().includes(searchQuery.value.toLowerCase())
  );
});

const displayedFilteredAds = computed(() => {
  const start = (currentPage.value - 1) * pageSize.value;
  const end = start + pageSize.value;
  return filteredAds.value.slice(start, end);
});

const toggleAddNewAd = () => {
  if (addNewAd.value) cancelAddNewAd();
  else handleAdd();
};
const cancelAddNewAd = () => {
  addNewAd.value = false;

  // 移除临时添加的广告（假设它总是在列表的第一位）
  if (ads.value.length > 0 && ads.value[0].ad_id.startsWith('new-')) {
    ads.value.shift();
  }

  // 如果正在编辑新广告，退出编辑模式
  if (editingKey.value && editingKey.value.startsWith('new-')) {
    editingKey.value = '';
    delete editableData[editingKey.value];
  }
};
const toggleDetails = (ad) => {
  selectedAd.value = selectedAd.value === ad ? null : ad;
  getEntireAdInfo(ad.ad_id)
};

const getAds = async () => {
  try {
    const res = await axios.post('/api/advertisement/GetAdvertisements');
    ads.value = res.data.advertisements.map(ad => ({...ad, showDetails: false}));
  } catch (error) {
    message.error(`获取广告列表失败：${error.message}`);
    console.error(error);
  }
};
const expandedRowKeys = ref([]);
const onExpand = async (expanded, record) => {
  if (expanded) {
    if (!expandedRowKeys.value.includes(record.ad_id)) {
      expandedRowKeys.value.push(record.ad_id);
    }
    if (!record.showDetails) {
      record.loading = true;
      record.showDetails = true;
      try {
        await getEntireAdInfo(record);
      } catch (err) {
        message.error(`获取广告详情失败，错误原因：${e}`)
      } finally {
        record.loading = false;

      }
    }
  } else {
    expandedRowKeys.value = expandedRowKeys.value.filter(key => key !== record.ad_id);
  }

}

async function getEntireAdInfo(record) {
  try {
    const res = await axios.post('api/advertisement/GetAdvertisementDetails', {
      ad_id: record.ad_id
    });
    const index = ads.value.findIndex(ad => ad.ad_id === record.ad_id);
    if (index !== -1) {
      ads.value[index] = {
        ...ads.value[index],
        show_count: res.data.ad.show_count,
        click_count: res.data.ad.click_count,
        click_statistics: res.data.click_statistics,
        loading: false,
        showDetails: true
      };
    }

  } catch (e) {
    console.error(e);
  }
}

const edit = (key) => {
  editingKey.value = key;
  editableData[key] = {
    ...ads.value.find(item => key === item.ad_id),
    imageInputType: 'url'  // 默认使用URL输入
  };
};

const save = async (key) => {
  try {
    const row = editableData[key];
    // 处理图片上传
    if (row.imageInputType === 'upload' && imageFile.value) {
      const formData = new FormData();
      formData.append('file', imageFile.value);
      const uploadResponse = await axios.post('/api/AdPicUpload/uploadLocal?type=Ad', formData, {
        headers: {
          'Content-Type': 'multipart/form-data'
        }
      });
      row.ad_picture = uploadResponse.data.url;
    }
    const adData = {
      ad_content: row.ad_content,
      ad_type: row.ad_type,
      start_time: row.start_time,
      end_time: row.end_time,
      ad_picture: row.ad_picture,
      ad_url: row.ad_url
    };
    let response;
    if (key.toString().startsWith('new-')) {
      // 新广告，使用POST请求
      response = await axios.post('/api/advertisement/AddAdvertisement', adData);
    } else {
      // 更新现有广告，使用PUT请求
      response = await axios.put('/api/advertisement/UpdateAdvertisement', {
        ad_id: key,
        ...adData
      });
    }

    if (response.status === 200) {
      const index = ads.value.findIndex(item => key === item.ad_id);
      Object.assign(ads.value[index], row);
      delete editableData[key];
      editingKey.value = '';
      message.success('广告更新成功');
    } else {
      throw new Error('Failed to update ad');
    }
  } catch (error) {
    console.error('Error updating ad:', error);
    message.error('更新广告失败，请稍后重试');
  }
};

const cancel = (key) => {
  delete editableData[key];
  editingKey.value = '';
};

const beforeUpload = (file) => {
  const isJpgOrPng = file.type === 'image/jpeg' || file.type === 'image/png';
  if (!isJpgOrPng) {
    message.error('只能上传 JPG/PNG 文件!');
  }
  const isLt2M = file.size / 1024 / 1024 < 2;
  if (!isLt2M) {
    message.error('上传的图片需要小于 2MB!');
  }
  return isJpgOrPng && isLt2M;
};
const imageFile = ref(null);
const handleImageChange = (info) => {
  imageFile.value = info.file.originFileObj;
  // if (info.file.status === 'done') {
  //   //editableData[editingKey.value].imageFile = info.file.originFileObj;
  //   editableData[editingKey.value].imageFile = info.target.files[0];
  //   message.success(`${info.file.name} file uploaded successfully`);
  // } else if (info.file.status === 'error') {
  //   message.error(`${info.file.name} file upload failed.`);
  // }
};

onMounted(async () => {
  await getAds();
});
</script>

<style scoped>
.ad-list-container {
  width: 100%;
  padding: 20px;
  margin-left: 0;
}

.title {
  text-align: center;
  margin-bottom: 30px;
  font-size: 2.5rem;
  font-weight: 300;
}

.action-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.search-container {
  flex-grow: 1;
  margin-left: 20px;
}

.add-ad-form {
  margin-bottom: 20px;
}

@media (max-width: 600px) {
  .action-bar {
    flex-direction: column;
    align-items: stretch;
  }

  .search-container {
    margin-left: 0;
    margin-top: 10px;
  }
}
</style>