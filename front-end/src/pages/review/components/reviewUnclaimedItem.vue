<script lang="ts" setup>
//从founditem表中返回所有没有审核的物品，在此处审核，可以查看物品详情，审核或驳回，审核成功修改物品状态，失败向用户发送邮件通知
import axios from 'axios';
import { onMounted, ref } from 'vue'

const baseURL = 'https://localhost:44343/api/';

const unreviewFoundItems = ref([])
const columns = [
    { title: '丢失物品', dataIndex: 'itemNameAndCategory' },
    { title: '物品描述', dataIndex: 'itemDescribe', ellipsis: true },
    { title: '发现地点', dataIndex: 'findPosition', ellipsis: true },
    { title: '发现时间', dataIndex: 'findTime' },
    { title: '物品标签', dataIndex: 'itemTags' },
    { title: '物品图片', dataIndex: 'itemImage' },
    { title: '通过', dataIndex: 'PASS' },
    { title: '驳回', dataIndex: 'REJECT' },
];

const getUnreviewFoundItems = async () => {
    const res = await axios.get(baseURL + '');
    unreviewFoundItems.value = res.result
}
const pass = async () => {
    //调用接口修改审核状态，修改后重新获取
    await axios.post(baseURL + '');
    getUnreviewFoundItems()
    alert("已通过！")
}
const reject = async () => {
    //调用接口删除lostitem中的信息，删除后重新获取
    await axios.post(baseURL + '');
    getUnreviewFoundItems()
    alert("已驳回！")
    //向用户发送邮件通知
    
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
                <h4>审核无主物品</h4>
            </div>
        </template>
        <template #bodyCell="{ column, record }">
            <template v-if="column.dataIndex === 'itemNameAndCategory'">
                <div class="text-title font-bold">
                    {{ record.itemName }}
                </div>
                <div class="text-subtext">
                    {{ record.itemCategory }}
                </div>
            </template>
            <template v-else-if="column.dataIndex === 'itemImage'">
                <img class="w-12 rounded" :src="record.itemImage" />
            </template>
            <template v-else-if="column.dataIndex === 'itemTags'">
                <span>
                    <a-tag v-for="tag in record.itemTags" :key="tag"
                        :color="tag === '贵重物品' ? 'volcano' : tag.length > 4 ? 'geekblue' : 'green'">
                        {{ tag.toUpperCase() }}
                    </a-tag>
                </span>
            </template>
            <template v-else-if="column.dataIndex === 'PASS'">
                <a-button type="primary" danger @click="pass">
                    <DeleteOutlined />
                    通过
                </a-button>
            </template>
            <template v-else-if="column.dataIndex === 'REJECT'">
                <a-button type="primary" danger @click="reject">
                    <DeleteOutlined />
                    驳回
                </a-button>
            </template>
        </template>
    </a-table>
</template>
