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
      <a-button type="primary" block @click="recharge" :loading="isLoading">立即充值</a-button>
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

  <a-button type="primary" @click="toggleRecharge">

    充值
  </a-button>
</template>

<script setup>

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

const selectedOption = ref(rechargeOptions[0]);
const isLoading = ref(false);
const errorMessage = ref('');
const rechargeResult = ref(null);
const isRecharged = ref(false);

const toggleRecharge = () => {
  isRecharged.value = !isRecharged.value;
}
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
</style>