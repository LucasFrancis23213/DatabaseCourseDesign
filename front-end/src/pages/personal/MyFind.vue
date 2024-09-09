<script lang="ts" setup>
  import { ref } from 'vue';
  import axios from 'axios';
  import { onMounted } from 'vue';
  axios.defaults.baseURL = import.meta.env.VITE_API_URL;
  import { useAccountStore } from '@/store/account';
  const {account, permissions} = useAccountStore();
  import ItemMap from "@/pages/admin_BasicFeature/ItemMap";
  const { categoryMapping } = ItemMap;

  const columns = [
  { title: '丢失物品', dataIndex: 'itemNameAndCategory' },
  { title: '物品描述', dataIndex: 'DESCRIPTION', ellipsis: true },
  { title: '发现地点', dataIndex: 'FOUND_LOCATION', ellipsis: true },
  { title: '发现时间', dataIndex: 'FOUND_DATE' },
  { title: '物品标签', dataIndex: 'TAG_ID' },
  { title: '物品图片', dataIndex: 'IMAGE_URL' },
  { title: '删除', dataIndex: 'delete'},
];

  const tagMapping = {
  1: '贵重物品',
  2: '私人用品',
  3: '医疗用品'
};


  const finds = ref([])
  const myItems = ref([])
  const getFinds = async () => {
  try {
    const res = await axios.get('api/QueryItem', {
      params: { 
            type: 1,
            review: 1
        }
    });

    finds.value = res.data.map(item => ({
      ...item,
      TAG_ID: [tagMapping[item.TAG_ID as keyof typeof tagMapping] || '未知'],
      CATEGORY_ID: categoryMapping[item.CATEGORY_ID as keyof typeof categoryMapping] || '未知类别',
    }));

    for (var find in finds.value) {
      if (finds.value[find].USER_ID == account.userId) {
        myItems.value.push(finds.value[find]);
      }
    }

    console.log('数据获取成功');

  } catch (error) {
    console.error('获取数据时出错:', error);
    // 可以在这里添加错误处理逻辑，比如设置一个错误状态
  }
}
  onMounted(() => getFinds())

  // TODO: 删除功能，获取当前行的物品id，通过物品id调用删除接口，更新最新的列表
  const iconLoading = ref<boolean>(false);
  const deleteOneLine = async(row) => {
    iconLoading.value = true;
    try {
        // 调用接口删除founditem中的信息，使用DELETE方法
        await axios.delete('api/DeleteItem', {
            data: JSON.stringify({ 
                ITEM_ID: row,
                type: 1
            }),
            headers: {
                'Content-Type': 'application/json'
            }
        });
        getFinds()
        alert("已删除！")
        // 向用户发送邮件通知
        // 注意：发送邮件的逻辑可能需要在后端实现
    } catch (error) {
        console.error('删除失败:', error);
        alert("删除失败，请重试！")
    }
  }
</script>

<template>
  <!-- 无主物品 -->
  <a-table :columns="columns" :dataSource="myItems" >
    <template #title>
      <div class="flex justify-between pr-4">
        <a-button type="primary"><h4>我发现的无主物品</h4> </a-button>
      </div>
    </template>
    <template #bodyCell="{ column, record }">
      <template v-if="column.dataIndex === 'itemNameAndCategory'">
        <div class="text-title font-bold">
          {{ record.ITEM_NAME }}
        </div>
        <div class="text-subtext">
          {{record.CATEGORY_ID}}
        </div>
      </template>
      <template v-else-if="column.dataIndex === 'IMAGE_URL'">
        <a-image class="w-12 rounded" :src="record.IMAGE_URL" />
      </template>
      <template v-else-if="column.dataIndex === 'TAG_ID'">
        <span>
          <a-tag
            v-for="tag in record.TAG_ID"
            :key="tag"
            :color="tag === '贵重物品' ? 'volcano' : tag.length > 4 ? 'geekblue' : 'green'"
            >
              {{ tag.toUpperCase() }}
          </a-tag>
        </span>
      </template>
      <template v-else-if="column.dataIndex === 'delete'">
        <a-button type="primary" danger :loading="iconLoading" @click="deleteOneLine(record.ITEM_ID)">
            <DeleteOutlined />
            删除此条
        </a-button>
      </template>
    </template>
  </a-table>
</template>
