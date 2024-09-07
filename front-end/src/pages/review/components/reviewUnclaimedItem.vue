<script lang="ts" setup>
//从founditem表中返回所有没有审核的物品，在此处审核，可以查看物品详情，审核或驳回，审核成功修改物品状态，失败向用户发送邮件通知
import axios from 'axios';
import { onMounted, ref } from 'vue'

const unreviewFoundItems = ref([])
const columns = [
    { title: '物品ID', dataIndex: 'ITEM_ID' },
    { title: '丢失物品', dataIndex: 'itemNameAndCategory' },
    { title: '物品描述', dataIndex: 'DESCRIPTION', ellipsis: true },
    { title: '发现地点', dataIndex: 'FOUND_LOCATION', ellipsis: true },
    { title: '发现时间', dataIndex: 'FOUND_DATE' },
    { title: '物品标签', dataIndex: 'TAG_ID' },
    { title: '物品图片', dataIndex: 'IMAGE_URL' },
    { title: '通过', dataIndex: 'PASS' },
    { title: '驳回', dataIndex: 'REJECT' },
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

const getUnreviewFoundItems = async () => {
    const res = await axios.get('api/QueryItem', {
        params: { type: 1 }
      });
      unreviewFoundItems.value = res.data.map(item => ({
      ...item,
      TAG_ID: [tagMapping[item.TAG_ID as keyof typeof tagMapping] || '未知'],
      CATEGORY_ID: categoryMapping[item.CATEGORY_ID as keyof typeof categoryMapping] || '未知类别'
    }));
}

const pass = async (ITEM_ID: string) => {
    console.log('通过的物品ID:', ITEM_ID);
    //调用接口修改审核状态，修改后重新获取
    await axios.post('api/PassItem', 
            JSON.stringify({ 
                ITEM_ID: ITEM_ID,
                type: 1
            }),
            {
                headers: {
                    'Content-Type': 'application/json'
                }
            }
        );
    getUnreviewFoundItems()
    alert("已通过！")
}
const reject = async (ITEM_ID: string) => {
    try {
        // 调用接口删除founditem中的信息，使用DELETE方法
        await axios.delete('api/DeleteItem', {
            data: JSON.stringify({ 
                ITEM_ID: ITEM_ID,
                type: 1
            }),
            headers: {
                'Content-Type': 'application/json'
            }
        });
        getUnreviewFoundItems()
        alert("已驳回！")
        // 向用户发送邮件通知
        // 注意：发送邮件的逻辑可能需要在后端实现
    } catch (error) {
        console.error('驳回失败:', error);
        alert("驳回失败，请重试！")
    }
    
}

onMounted(() => {
    getUnreviewFoundItems()
})

</script>
<template>
    <!-- 寻物启事审核区 -->
    <a-table :columns="columns" :dataSource="unreviewFoundItems">
        <template #title>
            <div class="flex justify-between pr-4">
                <h3 style="font-size: large;">审核无主物品</h3>
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
            <template v-else-if="column.dataIndex === 'PASS'">
                <a-button type="primary" danger @click="pass(record.ITEM_ID)">
                    <DeleteOutlined />
                    通过
                </a-button>
            </template>
            <template v-else-if="column.dataIndex === 'REJECT'">
                <a-button type="primary" danger @click="reject(record.ITEM_ID)">
                    <DeleteOutlined />
                    驳回
                </a-button>
            </template>
        </template>
    </a-table>
</template>
