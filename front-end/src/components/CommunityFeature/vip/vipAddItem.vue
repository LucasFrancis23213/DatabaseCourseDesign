<template>
  <div class="add-vip-form">
    <h2 class="form-title">新增VIP信息</h2>
    <form @submit.prevent="submitVip" class="vip-form">
      <div class="form-group">
        <label for="start_time">开始时间</label>
        <input
            id="start_time"
            type="datetime-local"
            v-model="vipData.start_time"
            required
        >
      </div>
      <div class="form-group">
        <label for="end_time">结束时间</label>
        <input
            id="end_time"
            type="datetime-local"
            v-model="vipData.end_time"
            required
        >
      </div>
      <div class="form-group">
        <label for="vip_status">vip状态</label>
        <select id="vip_status" v-model="vipData.vip_status" required>
          <option value="Active">有效</option>
          <option value="Inactive">无效</option>
          <option value="Cancelled">取消</option>
        </select>
      </div>
      <button type="submit" class="submit-btn" :disabled="isSubmitting">
        {{ isSubmitting ? '添加中...' : '添加VIP信息' }}
      </button>
    </form>
  </div>
</template>

<script setup>
import {ref, computed} from 'vue';
import axios from "axios";

axios.defaults.baseURL = import.meta.env.VITE_API_URL_VIP;

const emit = defineEmits(['vipAdded']);
const user_id = ref(123);

const vipData = ref({
  start_time: '',
  end_time: '',
  vip_status:'Inactive'
});

const isSubmitting = ref(false);

const isFormValid = computed(() => {
  return vipData.value.start_time && vipData.value.end_time;
});

const submitVip = async () => {
  if (!isFormValid.value) {
    alert('请填写所有必填字段');
    return;
  }

  isSubmitting.value = true;
  try {
    const res = await axios.post('/api/vip/AddVIPMember', {
      user_id: user_id.value,
      ...vipData.value
    });
    console.log(res);
    emit('vipAdded');
    resetForm();
    alert('VIP信息添加成功！');
  } catch (error) {
    console.error('添加VIP失败:', error);
    alert('添加VIP失败，请重试。');
  } finally {
    isSubmitting.value = false;
  }
};

const resetForm = () => {
  vipData.value = {
    start_time: '',
    end_time: ''
  };
};
</script>

<style scoped>
.add-vip-form {
  max-width: 500px;
  margin: 0 auto;
  padding: 20px;
  background-color: #f9f9f9;
  border-radius: 8px;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
}

.form-title {
  text-align: center;
  color: #333;
  margin-bottom: 20px;
}

.vip-form {
  display: flex;
  flex-direction: column;
}

.form-group {
  margin-bottom: 15px;
}

label {
  display: block;
  margin-bottom: 5px;
  color: #555;
  font-weight: bold;
}

input {
  width: 100%;
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 4px;
  font-size: 14px;
}

input:focus {
  outline: none;
  border-color: #4a90e2;
  box-shadow: 0 0 0 2px rgba(74, 144, 226, 0.2);
}

.submit-btn {
  background-color: #4a90e2;
  color: white;
  border: none;
  padding: 12px;
  border-radius: 4px;
  font-size: 16px;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.submit-btn:hover:not(:disabled) {
  background-color: #3a7bc8;
}

.submit-btn:disabled {
  background-color: #a0a0a0;
  cursor: not-allowed;
}

@media (max-width: 600px) {
  .add-vip-form {
    padding: 15px;
  }
}
</style>