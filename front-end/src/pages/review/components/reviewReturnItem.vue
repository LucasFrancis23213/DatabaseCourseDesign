<script lang="ts" setup>
//审核认领需要改变丢失物品和无主物品状态，发现者获得赏金。
import axios from 'axios';
import { onMounted, ref } from 'vue'

const baseURL = 'http://121.36.200.128:5000/api/';

const unreviewClaimItems = ref([])
const columns = [
    { title: '丢失物品', dataIndex: 'itemNameAndCategory' },
    { title: '物品图片', dataIndex: 'IMAGE_URL' },
    { title: '认领者', dataIndex: 'LOST_USER'},
    { title: '发现者', dataIndex: 'FIND_USER'},
    { title: '通过', dataIndex: 'PASS' },
    { title: '驳回', dataIndex: 'REJECT' },
];

const getUnreviewClaimItems = async () => {
    const res = await axios.get(baseURL + '');
    unreviewClaimItems.value = res.result
}
const pass = async () => {
//调用接口修改物品审核状态，并给与发现者赏金，修改后重新获取
    await axios.post(baseURL + '');
    getUnreviewClaimItems()
    alert("已通过！")
}
const reject = async () => {
//调用接口删除lostitem中的信息，删除后重新获取
    await axios.post(baseURL + '');
    getUnreviewClaimItems()
    alert("已驳回！")
//向用户发送邮件通知

}

onMounted(() => {
    getUnreviewClaimItems()
})

</script>
<template>
    <!-- 寻物启事审核区 -->
    <a-table :columns="columns" :dataSource="unreviewClaimItems">
        <template #title>
            <div class="flex justify-between pr-4">
                <h3 style="font-size: large;">审核失物认领</h3>
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
            <template v-else-if="column.dataIndex === 'LOST_USER'">
                <div class="text-title font-bold">
                    {{ record.LOST_USER_NAME }}
                </div>
            </template>
            <template v-else-if="column.dataIndex === 'FIND_USER'">
                <div class="text-title font-bold">
                    {{ record.FIND_USER_NAME }}
                </div>
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