<script lang="ts" setup>
//从lostitem表中返回所有没有审核的物品，在此处审核，可以审核或驳回，审核成功修改物品状态，审核不通过向用户发送邮件通知
import axios from 'axios';
import { onMounted, ref } from 'vue'

const baseURL = 'http://121.36.200.128:5000/api/';

const unreviewLostItems = ref([])
const columns = [
    { title: '丢失物品', dataIndex: 'itemNameAndCategory' },
    { title: '物品描述', dataIndex: 'DESCRIPTION', ellipsis: true },
    { title: '遗失地点', dataIndex: 'LOST_LOCATION', ellipsis: true },
    { title: '丢失时间', dataIndex: 'LOST_DATE' },
    { title: '物品标签', dataIndex: 'TAG_ID' },
    { title: '物品图片', dataIndex: 'IMAGE_URL' },
    { title: '是否悬赏', dataIndex: 'IS_REWARDED' },
    { title: '悬赏金额', dataIndex: 'REWARD_AMOUNT' },
    { title: '截止时间', dataIndex: 'DEADLINE' },
    { title: '通过', dataIndex: 'PASS' },
    { title: '驳回', dataIndex: 'REJECT' },
];
const IS_REWARDEDDict = {
    0: '未悬赏',
    1: '悬赏',
};


const getUnreviewLostItems = async () => {
    const res = await axios.get(baseURL + '');
    unreviewLostItems.value = res.result
}
const pass = async () => {
//调用接口修改审核状态，修改后重新获取
    await axios.post(baseURL + '');
    getUnreviewLostItems()
    alert("已通过！")
}
const reject = async () => {
//调用接口删除lostitem中的信息，删除后重新获取
    await axios.post(baseURL + '');
    getUnreviewLostItems()
    alert("已驳回！")
//向用户发送邮件通知

}

onMounted(() => {
    getUnreviewLostItems()
})

</script>
<template>
    <!-- 寻物启事审核区 -->
    <a-table :columns="columns" :dataSource="unreviewLostItems">
        <template #title>
            <div class="flex justify-between pr-4">
                <h3 style="font-size: large;">审核寻物启事</h3>
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
            <template v-else-if="column.dataIndex === 'IS_REWARDED'">
                <a-badge class="text-subtext" :color="'green'">
                    <template #text>
                        <span class="text-subtext">{{ IS_REWARDEDDict[Number(record.IS_REWARDED)] }}</span>
                    </template>
                </a-badge>
            </template>
            <template v-else-if="column.dataIndex === 'TAG_ID'">
                <span>
                    <a-tag v-for="tag in record.TAG_ID" :key="tag"
                        :color="tag === '贵重物品' ? 'volcano' : tag.length > 4 ? 'geekblue' : 'green'">
                        {{ tag }}
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