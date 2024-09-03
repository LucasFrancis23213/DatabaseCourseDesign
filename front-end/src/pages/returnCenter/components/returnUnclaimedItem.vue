<script lang="ts" setup>
//从founditem表中返回所有没有审核的物品，在此处审核，可以查看物品详情，审核或驳回，审核成功修改物品状态，失败向用户发送邮件通知
import axios from 'axios';
import { onMounted, ref } from 'vue'

const baseURL = 'http://121.36.200.128:5174/api/';

const returnFounds = ref([])
const columns = [
    { title: '丢失物品', dataIndex: 'itemNameAndCategory' },
    { title: '物品描述', dataIndex: 'DESCRIPTION', ellipsis: true },
    { title: '发现地点', dataIndex: 'FOUND_LOCATION', ellipsis: true },
    { title: '发现时间', dataIndex: 'FOUND_DATE' },
    { title: '物品标签', dataIndex: 'TAG_ID' },
    { title: '物品图片', dataIndex: 'IMAGE_URL' },
    { title: '签署认领协议', dataIndex: 'SIGN' },
    { title: '取消认领', dataIndex: 'REJECT' },
];

const tagMapping = {
  1: '贵重物品',
  2: '私人用品',
  3: '医疗用品'
};

const categoryMapping = {
  '1': '物品类别1',
  '2': '手表',
};

const getFinds = async () => {
  try {
    const res = await axios.get(baseURL + 'QueryItem', {
      params: { 
            type: 1,
            review: 1
        }
    });

    returnFounds.value = res.data.map(item => ({
      ...item,
      TAG_ID: [tagMapping[item.TAG_ID as keyof typeof tagMapping] || '未知'],
      CATEGORY_ID: categoryMapping[item.CATEGORY_ID as keyof typeof categoryMapping] || '未知类别',
    }));

    console.log('数据获取成功');

  } catch (error) {
    console.error('获取数据时出错:', error);
    // 可以在这里添加错误处理逻辑，比如设置一个错误状态
  }
}
onMounted(() => getFinds())

</script>
<template>
    <!-- 寻物启事审核区 -->
    <a-table :columns="columns" :dataSource="returnFounds">
        <template #title>
            <div class="flex justify-between pr-4">
                <h3 style="font-size: large;">认领/归还自己发现的遗失物品（无主物品）</h3>
            </div>
        </template>
        <template #bodyCell="{ column, record }">
            <template v-if="column.dataIndex === 'itemNameAndCategory'">
                <div class="text-title font-bold">
                    {{ record.ITEM_NAME }}
                </div>
                <div class="text-subtext">
                    {{ record.CATEGORY_ID }}
                </div>
            </template>
            <template v-else-if="column.dataIndex === 'IMAGE_URL'">
                <img class="w-12 rounded" :src="record.IMAGE_URL" />
            </template>
            <template v-else-if="column.dataIndex === 'TAG_ID'">
                <span>
                    <a-tag v-for="tag in record.TAG_ID" :key="tag"
                        :color="tag === '贵重物品' ? 'volcano' : tag.length > 4 ? 'geekblue' : 'green'">
                        {{ tag.toUpperCase() }}
                    </a-tag>
                </span>
            </template>
            <template v-else-if="column.dataIndex === 'SIGN'">
                <a-button type="primary" danger @click="">
                    <DeleteOutlined />
                    签署认领协议
                </a-button>
            </template>
        </template>
    </a-table>
</template>
