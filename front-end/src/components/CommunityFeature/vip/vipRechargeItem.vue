<template>
  <div class="vip-recharge">
    <h2>VIP会员充值</h2>
    <div class="recharge-options">
      <div
        v-for="option in rechargeOptions"
        :key="option.id"
        @click="selectOption(option)"
        :class="['option', { 'selected': selectedOption === option }]"
      >
        <h3>{{ option.name }}</h3>
        <p class="price">¥{{ option.price }}</p>
        <p class="duration">{{ option.months }}个月</p>
      </div>
    </div>
    <button @click="recharge" :disabled="!canRecharge" class="recharge-btn">
      立即充值
    </button>
    <div v-if="rechargeResult" class="recharge-result">
      <h3>充值成功</h3>
      <p><strong>订单ID:</strong> {{ rechargeResult.order_id }}</p>
      <p><strong>总金额:</strong> ¥{{ rechargeResult.total_amount }}</p>
      <p><strong>积分返还:</strong> {{ rechargeResult.point_return }}</p>
      <p><strong>VIP开始日期:</strong> {{ formatDate(rechargeResult.vip_start_date) }}</p>
      <p><strong>VIP结束日期:</strong> {{ formatDate(rechargeResult.vip_end_date) }}</p>
      <p><strong>VIP状态:</strong> {{ rechargeResult.vip_status }}</p>
    </div>
    <p v-if="errorMessage" class="error-message">{{ errorMessage }}</p>
  </div>
</template>

<style scoped>
.vip-recharge {
  max-width: 800px;
  margin: 0 auto;
  padding: 40px;
  background-color: #f9f9f9;
  border-radius: 10px;
  box-shadow: 0 0 20px rgba(0, 0, 0, 0.1);
}

h2 {
  color: #333;
  font-size: 28px;
  margin-bottom: 30px;
  text-align: center;
}

.recharge-options {
  display: flex;
  justify-content: space-between;
  margin-bottom: 30px;
}

.option {
  flex: 1;
  background-color: #fff;
  border: 2px solid #e0e0e0;
  border-radius: 8px;
  padding: 20px;
  text-align: center;
  cursor: pointer;
  transition: all 0.3s ease;
  margin: 0 10px;
}

.option:hover {
  transform: translateY(-5px);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
}


.option.selected {
  border-color: #4CAF50;
  background-color: #e8f5e9;
}

.option h3 {
  color: #333;
  font-size: 20px;
  margin-bottom: 10px;
}

.option .price {
  font-size: 24px;
  font-weight: bold;
  color: #4CAF50;
  margin-bottom: 5px;
}

.option .duration {
  color: #666;
}

.recharge-btn {
  display: block;
  width: 100%;
  padding: 15px;
  background-color: #4CAF50;
  color: white;
  border: none;
  border-radius: 5px;
  font-size: 18px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.recharge-btn:hover {
  background-color: #45a049;
}

.recharge-btn:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}

.recharge-result {
  background-color: #e8f5e9;
  border: 1px solid #4CAF50;
  border-radius: 5px;
  padding: 20px;
  margin-top: 30px;
}

.recharge-result h3 {
  color: #4CAF50;
  margin-bottom: 15px;
}

.recharge-result p {
  margin: 10px 0;
  color: #333;
}

.error-message {
  color: #f44336;
  text-align: center;
  margin-top: 20px;
  font-weight: bold;
}
</style>
<script setup>
import { ref, computed } from 'vue'
import axios from 'axios'
axios.defaults.baseURL=import.meta.env.VITE_API_URL;
const rechargeOptions = [
  { id: 1, name: '月度VIP', price: 30, months: 1 },
  { id: 2, name: '季度VIP', price: 80, months: 3 },
  { id: 3, name: '年度VIP', price: 280, months: 12 },
]

const selectedOption = ref(null)
const rechargeResult = ref(null)
const errorMessage = ref('')

const selectOption = (option) => {
  selectedOption.value = option
}

const canRecharge = computed(() => {
  return selectedOption.value !== null
})

const recharge = async () => {
  if (canRecharge.value) {
    try {
      const response = await axios.post('api/vip/RechargeVip', {
        user_id: 2, // 这里应该是实际的用户ID，可能需要从用户状态或登录信息中获取
        recharge_time: selectedOption.value.months,
        total_amount:selectedOption.value.price
      })

      if (response.data.status === 'success') {
        rechargeResult.value = response.data
        errorMessage.value = ''
      } else {
        errorMessage.value = '充值失败，请稍后重试'
      }
    } catch (error) {
      console.error('充值出错:', error)
      errorMessage.value = '充值过程中出现错误，请稍后重试'
    }
  }
}

const formatDate = (dateString) => {
  return new Date(dateString).toLocaleString()
}
</script>
