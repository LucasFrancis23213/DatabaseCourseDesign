<script lang="ts" setup>
  import { ref, onMounted } from 'vue';
  import axios from 'axios';
  import { message } from 'ant-design-vue';

  const columns = [
    { title: '丢失物品', dataIndex: 'itemNameAndCategory'},
    { title: '物品描述', dataIndex: 'Description', ellipsis: true},
    { title: '遗失地点', dataIndex: 'Lost_Location', ellipsis: true},
    { title: '丢失时间', dataIndex: 'Lost_Date' },
    { title: '物品标签', dataIndex: 'Tag_ID' },
    { title: '物品图片', dataIndex: 'Image_URL' },
    { title: '是否悬赏', dataIndex: 'isRewarded' },
    { title: '悬赏金额', dataIndex: 'Reward_Amount' },
    { title: '截止时间', dataIndex: 'Deadline' },
    { title: '删除', dataIndex: 'delete'},
  ];

  type IsRewarded = 0 | 1;
  const IsRewardedDict = {
    0: '未悬赏',
    1: '悬赏',
  };

  const publishs = ref([]);

  const getPublishs = async () => {
    const res = await axios.get('api/publishs');
    publishs.value = res.data;
  }

  onMounted(() => getPublishs());

  // TODO: 删除功能，获取当前行的物品id，通过物品id调用删除接口，更新最新的列表
  const iconLoading = ref<boolean>(false);
  const deleteOneLine = async(row) => {
    iconLoading.value = true;
    await axios.delete(`/del/${row}`)
    getPublishs()
    setTimeout(() => {
      message.success('删除成功！');
      iconLoading.value = false;
    }, 1000);
  }
</script>

<template>
  <!-- 寻物启事 -->
  <a-table :columns="columns" :dataSource="publishs" >
    <template #title>
      <div class="flex justify-between pr-4">
        <h4>我的寻物启事</h4>
      </div>
    </template>
    <template #bodyCell="{ column, record }">
      <template v-if="column.dataIndex === 'itemNameAndCategory'">
        <div class="text-title font-bold">
          {{ record.Item_Name }}
        </div>
        <div class="text-subtext">
          {{record.Category_ID}}
        </div>
      </template>
      <template v-else-if="column.dataIndex === 'Image_URL'">
        <img class="w-12 rounded" :src="record.Image_URL" />
      </template>
      <template v-else-if="column.dataIndex === 'isRewarded'">
        <a-badge class="text-subtext" :color="'green'">
          <template #text>
            <span class="text-subtext">{{ IsRewardedDict[Number(record.isRewarded) as IsRewarded] }}</span>
          </template>
        </a-badge>
      </template>
      <template v-else-if="column.dataIndex === 'Tag_ID'">
        <span>
          <a-tag
            v-for="tag in record.Tag_ID"
            :key="tag"
            :color="tag === '贵重物品' ? 'volcano' : tag.length > 4 ? 'geekblue' : 'green'"
            >
              {{ tag.toUpperCase() }}
          </a-tag>
        </span>
      </template>
      <template v-else-if="column.dataIndex === 'delete'">
        <a-button type="primary" danger :loading="iconLoading" @click="deleteOneLine()">
            <DeleteOutlined />
            删除此条
        </a-button>
      </template>
    </template>
  </a-table>
</template>
