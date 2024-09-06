<template>
  <a-modal v-model:visible="isRecharged" title="VIP会员充值">
    <template #footer></template>
    <a-form :label-col="{ span: 8 }" :wrapper-col="{ span: 16 }">
      <a-radio-group v-model:value="selectedOption" class="a-radio-group">
        <a-radio-button v-for="option in rechargeOptions" :key="option.id" :value="option.id" class="option"
                        :class="['option', { selected: selectedOption === option.id }]">
          <span>{{ option.name }}</span>
          <span> ¥{{ option.price }}</span>
          <span> {{ option.months }}个月</span>
        </a-radio-button>
      </a-radio-group>
      <a-button type="primary" block @click="generateQRCode" :loading="isLoading">立即充值</a-button>
      <qrcode-vue v-if="needQRCode" class="qrcode mx-auto" :value="qrCodeUrl" :size="200" level="H"></qrcode-vue>
      <a-alert v-if="errorMessage" message={errorMessage} type="error" show-icon/>
      <a-result v-if="rechargeResult" status="success" title="充值成功">
        <template #extra>
          <a-button type="primary" @click="toggleRecharge">返回首页</a-button>
        </template>
        <div class="">订单ID: {{ rechargeResult.order_id }}</div>
        <div>总金额: ¥{{ rechargeResult.total_amount }}</div>
      </a-result>
    </a-form>
  </a-modal>
  <a-button type="primary" @click="toggleRecharge">充值</a-button>
</template>

<script setup>

import {ref, computed, onUnmounted, onMounted} from 'vue';
import {useAccountStore} from '@/store/account';

const {account, permissions} = useAccountStore();

import {message} from 'ant-design-vue';
import axios from 'axios'
import QrcodeVue from "qrcode.vue";

axios.defaults.baseURL = import.meta.env.VITE_API_URL;
const qrCodeUrl = ref(`${axios.defaults.baseURL}/api/`)

const rechargeOptions = [
  {id: 1, name: '月度VIP', price: 30, months: 1},
  {id: 2, name: '季度VIP', price: 80, months: 3},
  {id: 3, name: '年度VIP', price: 280, months: 12},
];

const selectedOption = ref(rechargeOptions[0]);
const isLoading = ref(false);
const errorMessage = ref('');
const rechargeResult = ref(null);
const isRecharged = ref(false);
const needQRCode =ref(false);
const rechargeId = ref('');
const toggleRecharge = () => {
  isRecharged.value = !isRecharged.value;
}

const toggleNeedQRcode =()=>{
  needQRCode.value=!needQRCode.value;
}
const generateQRCode=()=>{
  toggleNeedQRcode();
  const selected = rechargeOptions.find(option => option.id === selectedOption.value);
  rechargeId.value= 'recharge_'+account.userId + Date.now();

  qrCodeUrl.value=`localhost:5173/#/recharge/${rechargeId.value}`
  console.log(qrCodeUrl.value)
  //setupWebSocket();
  ws.value.send(JSON.stringify({ type: 'start_monitoring', rechargeId: rechargeId.value, userId:+account.userId}));
}
const ws=ref();
const setupWebSocket = () => {
  ws.value = new WebSocket(`wss://localhost:44343/rechargews?user_id=${account.userId}`);

  ws.value.onopen = () => {
    console.log('WebSocket连接已建立');
    //ws.value.send({ type: 'start_monitoring', rechargeId: +rechargeId.value, userId:+account.userId});
  };

  ws.value.onmessage = (event) => {
    console.log(event)
    console.log(event.data)
    if (event.data === 'Recharge Success') {
      //rechargeStatus.value = '充值成功！余额已更新';
      // 这里可以添加更新用户余额的逻辑
    }
    recharge();
    disconnection();
    toggleNeedQRcode();
  };

  ws.value.onerror = (error) => {
    console.error('WebSocket错误:', error);
    //rechargeStatus.value = '连接错误，请稍后重试';
    disconnection();
  };

  ws.value.onclose = (e) => {
    console.log(e)
    console.log('WebSocket连接已关闭');
  };
};

const disconnection =()=>{
  if (ws.value) {
    ws.value.close();
    ws.value = null;
  }
}

onMounted(()=>{
  setupWebSocket()
})
onUnmounted(()=>{
  disconnection()
})
const recharge = async () => {
  try {

    const selected = rechargeOptions.find(option => option.id === selectedOption.value);
    console.log(selected)
    const response = await axios.post('api/vip/RechargeVip', {
      user_id: account.userId,
      recharge_time: selected.months,
      total_amount: selected.price
    });
    rechargeResult.value = response.data;
    message.success('充值成功');
  } catch (error) {
    message.error('充值失败');
  } finally {
    isLoading.value = false;
  }
};
</script>

<style lang="less" scoped>
.a-radio-group {
  display: flex;
  justify-content: space-between;
  width: 100%;
}

.option {
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  background-color: #fff;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  padding: 15px;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s ease;
  margin: 5px;
  height: 120px; /* 调整高度以适应内容 */
}

.option span {
  display: block;
  margin-bottom: 5px;
}

.option:hover {
  transform: translateY(-5px);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
}

.option.selected {
  border-color: #1890ff;
  background-color: #e6f7ff;
  color: #1890ff;
  font-weight: bold;
  box-shadow: 0 0 10px rgba(24, 144, 255, 0.3);
}

.option.selected::after {
  content: '\2714';
  position: absolute;
  top: 5px;
  right: 5px;
  font-size: 16px;
}

.qrcode{
  display: flex;
  margin-top: 10px;
}
</style>