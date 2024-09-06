<template>
  <a-card title="VIP会员充值" :bordered="false">
    <a-form :label-col="{ span: 8 }" :wrapper-col="{ span: 16 }">
      <a-radio-group v-model:value="selectedOption">
        <a-radio-button v-for="option in rechargeOptions" :key="option.id" :value="option">
          {{ option.name }} (¥{{ option.price }}, {{ option.months }}个月)
        </a-radio-button>
      </a-radio-group>
      <a-button type="primary" block @click="recharge" :loading="isLoading">立即充值</a-button>
      <a-alert v-if="errorMessage" message={errorMessage} type="error" show-icon/>
      <a-result v-if="rechargeResult" status="success" title="充值成功">
        <template #extra>
          <a-button type="primary">返回首页</a-button>
        </template>
        <div>订单ID: {{ rechargeResult.order_id }}</div>
        <div>总金额: ¥{{ rechargeResult.total_amount }}</div>
      </a-result>
    </a-form>
  </a-card>
<template>
  <a-space direction="vertical" align="center">
    <a-qrcode :value="text" />
    <a-input v-model:value="text" placeholder="-" :maxlength="60" />
  </a-space>
</template></template>

<script setup>
const text = ref('https://www.antdv.com/');
import {ref, computed} from 'vue';
import {useAccountStore} from '@/store/account';

const {account, permissions} = useAccountStore();

import {message} from 'ant-design-vue';
import axios from 'axios'

axios.defaults.baseURL = import.meta.env.VITE_API_URL;


const rechargeOptions = [
  {id: 1, name: '月度VIP', price: 30, months: 1},
  {id: 2, name: '季度VIP', price: 80, months: 3},
  {id: 3, name: '年度VIP', price: 280, months: 12},
];

const selectedOption = ref();
const isLoading = ref(false);
const errorMessage = ref('');
const rechargeResult = ref(null)

const recharge = async () => {
  try {
    console.log(selectedOption.value)

    const response = await axios.post('api/vip/RechargeVip', {
      user_id: account.userId,
      recharge_time: selectedOption.value.months,
      total_amount: selectedOption.value.price
    });
    rechargeResult.value=response.data;
    message.success('充值成功');
  } catch (error) {
    message.error('充值失败');
  } finally {
    isLoading.value=false;
  }
};
</script>