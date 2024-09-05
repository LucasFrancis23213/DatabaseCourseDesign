<script lang="ts" setup>
import axios from 'axios';
import { onMounted, ref } from 'vue'
import { useAccountStore } from '@/store/account';
const {account} = useAccountStore();

const baseURL = 'https://localhost:44343/api/';

const returnLosts = ref([])

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
    { title: '签署协议', dataIndex: 'SIGN' },
    { title: '取消', dataIndex: 'REJECT' },
];
const IS_REWARDEDDict = {
    0: '未悬赏',
    1: '悬赏',
};

const tagMapping = {
  1: '贵重物品',
  2: '私人用品',
  3: '医疗用品'
};

const categoryMapping = {
  '1': '物品类别1',
  '2': '手表',
};

const getPublishs = async () => {
    console.log(account);
    try {
        const res = await axios.get(baseURL + 'claim/QueryItem', {
            params: {
                type: 0,
                userID: account.userId,
            }
        });
        const items = res.data;

        // Fetch signing status for each item
        const fetchSignStatusPromises = items.map(async item => {
            try {
                const signRes = await axios.get(baseURL + 'claim/CheckSign', {
                    params: {
                        userID: +account.userId,
                        itemID: item.ITEM_ID,
                    }
                }); 
                
                item.isSigned = signRes.data.item2.item1; // 假设后端返回的签署状态在这个位置
                console.log('Sign status:', item.isSigned);
            } catch (error) {
                console.error('Error checking sign status:', error);
                item.isSigned = false; // Default to false if there's an error
            }
            return item;
        });

        // Wait for all sign status checks to complete
        const itemsWithSignStatus = await Promise.all(fetchSignStatusPromises);

        returnLosts.value = itemsWithSignStatus.map(item => ({
            ...item,
            TAG_ID: [tagMapping[item.TAG_ID as keyof typeof tagMapping] || '未知'],
            CATEGORY_ID: categoryMapping[item.CATEGORY_ID as keyof typeof categoryMapping] || '未知类别'
        }));

        console.log('数据获取成功');
    } catch (error) {
        console.error('获取数据时出错:', error);
    }
};

onMounted(() => getPublishs());

// 新增：控制模态框显示的响应式变量
const isModalVisible = ref(false);
const agreementChecked = ref(false);
const selectedItemId = ref<string | null>(null);

// 新增：显示模态框的方法
const showModal = (itemID : string) => {
  isModalVisible.value = true;
  selectedItemId.value = itemID;
};

// 新增：处理模态框确认的方法
const handleOk = async () => {
  if (agreementChecked.value && selectedItemId.value) {
    var jsonFormData = JSON.stringify({
      userID: +account.userId,
      itemID: selectedItemId.value,
    });
    await axios.post(baseURL + 'claim/SignAgreement', jsonFormData, {
          headers: {
            'Content-Type': 'application/json',
          },
        });
    alert('协议已签署！');
    isModalVisible.value = false;
    agreementChecked.value = false; 
    selectedItemId.value = null; 

    // Update signing status in the list after signing
    const signedItem = returnLosts.value.find(item => item.ITEM_ID === selectedItemId.value);
    if (signedItem) signedItem.isSigned = true;
    window.location.reload();
  } else {
    // 如果没有勾选同意协议，可以给出提示
    alert('请先阅读并同意协议');
  }
};

// 新增：处理模态框取消的方法
const handleCancel = () => {
  isModalVisible.value = false;
  agreementChecked.value = false; 
  selectedItemId.value = null; 
};

// 新增：控制取消确认模态框显示的响应式变量
const isCancelModalVisible = ref(false);
const cancelItemId = ref<string | null>(null);

// 新增：显示取消确认模态框的方法
const showCancelModal = (itemID: string) => {
  isCancelModalVisible.value = true;
  cancelItemId.value = itemID;
};

// 新增：处理取消确认的方法
const handleCancelConfirm = async () => {
  if (cancelItemId.value) {
    try {
      await axios.delete(baseURL + 'claim/DeleteClaim', {
        params: {
          itemID: cancelItemId.value,
        },
      });
      alert('操作已取消！');
      isCancelModalVisible.value = false;
      cancelItemId.value = null;
      // 刷新数据
      await getPublishs();
    } catch (error) {
      console.error('取消操作失败:', error);
      alert('取消操作失败，请重试。');
    }
  }
};

// 新增：处理取消确认模态框关闭的方法
const handleCancelModalClose = () => {
  isCancelModalVisible.value = false;
  cancelItemId.value = null;
};

</script>

<template>
  <!-- 寻物启事审核区 -->
  <a-table :columns="columns" :dataSource="returnLosts">
    <template #title>
      <div class="flex justify-between pr-4">
        <h3 style="font-size: large;">归还/认领他人找到的遗失物品（寻物启事）</h3>
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
      <template v-if="column.dataIndex === 'SIGN'">
        <template v-if="record.isSigned">
          <span><a-badge status="success" /> 已签署</span>
        </template>
        <template v-else>
          <a-button type="primary" @click="showModal(record.ITEM_ID)">
            签署协议
          </a-button>
        </template>
      </template>
      <template v-else-if="column.dataIndex === 'REJECT'">
        <a-button type="primary" danger @click="showCancelModal(record.ITEM_ID)">
          取消
        </a-button>
      </template>
    </template>
  </a-table>

  <!-- 新增：协议模态框 -->
  <a-modal v-model:visible="isModalVisible" title="签署协议" @ok="handleOk" @cancel="handleCancel" width="800px">
    <div style="max-height: 400px; overflow-y: auto;">
      <h2 style="text-align: center;">寻觅有道平台认领及归还物品协议</h2>

      <p>本协议由物品认领人（以下简称"认领人"）、物品归还人（以下简称"归还人"）与寻觅有道平台（以下简称"平台"）共同缔结，具有法律约束力。</p>

      <h3>1. 协议目的</h3>
      <p>本协议旨在规范认领人及归还人通过平台认领他人找到的遗失物品、归还自己发现的无主物品的过程，确保物品归还的公平、合法和有序进行。</p>

      <h3>2. 认领人及归还人声明与保证</h3>
      <p>2.1 认领人及归还人保证所提供的个人信息和物品信息真实、准确、完整。</p>
      <p>2.2 认领人及归还人承诺是遗失物品的合法所有人或授权代表。</p>
      <p>2.3 认领人及归还人同意配合平台进行必要的身份验证和物品所有权确认。</p>

      <h3>3. 认领流程</h3>
      <p>3.1 认领人应详细描述遗失物品的特征，包括但不限于物品类型、外观、内容等。</p>
      <p>3.2 平台将根据认领人提供的信息与登记的失物信息进行匹配。</p>
      <p>3.3 如匹配成功，平台将安排认领人与拾得人进行物品确认。</p>
      <p>3.4 认领人需在平台规定的时间内完成认领，否则视为放弃认领权。</p>

      <h3>4. 物品交接</h3>
      <p>4.1 物品交接应在平台指定或双方约定的安全场所进行。</p>
      <p>4.2 认领人在接收物品时应仔细检查，确认物品状态，并签署交接确认书。</p>
      <p>4.3 如物品有损坏，认领人应立即向平台报告。</p>

      <h3>5. 费用与奖励</h3>
      <p>5.1 认领人同意支付平台规定的手续费（如有）。</p>
      <p>5.2 如物品有悬赏，认领人同意按照事先约定的金额支付给拾得人。</p>
      <p>5.3 认领人可自愿给予拾得人适当感谢，但不得以此要求额外服务或优待。</p>

      <h3>6. 免责声明</h3>
      <p>6.1 平台仅提供信息对接服务，不对物品的保管状况承担责任。</p>
      <p>6.2 因不可抗力导致的物品损坏或丢失，平台不承担赔偿责任。</p>

      <h3>7. 争议解决</h3>
      <p>7.1 本协议履行过程中发生的任何争议，双方应友好协商解决。</p>
      <p>7.2 协商不成的，任何一方可向平台所在地有管辖权的人民法院提起诉讼。</p>

      <h3>8. 协议生效</h3>
      <p>8.1 认领人及归还人点击"我已阅读并同意协议"按钮即视为已阅读、理解并同意本协议的全部内容。</p>
      <p>8.2 本协议自认领人及归还人均确认同意之日起生效。</p>
    </div>

    <!-- 使用 a-row 和 a-col 来居中显示复选框 -->
    <a-row type="flex" justify="center" align="middle" style="margin-top: 20px;">
      <a-col>
        <a-checkbox v-model:checked="agreementChecked">我已阅读并同意协议</a-checkbox>
      </a-col>
    </a-row>

    <template #footer>
      <a-button key="back" @click="handleCancel">返回</a-button>
      <a-button key="submit" type="primary" :disabled="!agreementChecked" @click="handleOk">
        签署
      </a-button>
    </template>
  </a-modal>

  <!-- 新增：取消确认模态框 -->
  <a-modal v-model:visible="isCancelModalVisible" title="取消流程" :footer="null" @cancel="handleCancelModalClose"
    width="400px">
    <div style="text-align: center;">
      <p style="color: red; font-size: 16px; margin-bottom: 0px;">
        你确定要将物品退回寻物启事,
      </p>
      <p style="color: red; font-size: 16px; margin-bottom: 20px;">
        并取消目前流程吗？
      </p>
      <a-space>
        <a-button type="primary" @click="handleCancelModalClose">
          返回
        </a-button>
        <a-button @click="handleCancelConfirm" style="color: black; border-color: black;">
          确认
        </a-button>
      </a-space>
    </div>
  </a-modal>
</template>