<template>
  <div class="add-vip-form">
    <h2 class="form-title">新增VIP信息</h2>
    <form @submit.prevent="submitVip" class="vip-form">
      <div class="form-group">
        <label for="end_time">结束时间</label>
        <input
            id="end_time"
            type="datetime-local"
            v-model="vipData.vip_end_time"
            required
        >
      </div>
      <div class="form-group">
        <label for="vip_status">vip状态</label>
        <Select id="vip_status" v-model="vipData.vip_status" required>
          <Select.Option value="Active">有效</Select.Option>
          <Select.Option value="Inactive">无效</Select.Option>
          <Select.Option value="Cancelled">取消</Select.Option>
        </Select>
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
import {message} from "ant-design-vue";
import {Select} from "ant-design-vue";

axios.defaults.baseURL = import.meta.env.VITE_API_URL;
const props = defineProps(['user_id']);
const emit = defineEmits(['vipAdded']);


const vipData = ref({
  vip_start_time: '',
  vip_end_time: '',
  vip_status:'Active'
});

const isSubmitting = ref(false);

const isFormValid = computed(() => {
  return vipData.value.vip_end_time && 1;
});

function validateTime() {
  return vipData.value.vip_end_time > vipData.value.vip_start_time;
}

const submitVip = async () => {
  if (!isFormValid.value) {
    alert('请填写所有必填字段');
    return;
  }
  if (!validateTime()) {
      message.error('vip结束时间必须晚于开始时间');
      return;
    }
  isSubmitting.value = true;
  try {
    vipData.value.vip_start_time = new Date().toISOString();
    const res = await axios.post('/api/vip/AddVIPMember', {
      user_id: props.user_id,
      ...vipData.value
    });
    console.log(res);
    emit('vipAdded');
    resetForm();
    message.success('VIP信息添加成功！');
  } catch (error) {
    //console.error('添加VIP失败:', error);
    alert('添加VIP失败，请重试或者检查该用户是否已有vip');
  } finally {
    isSubmitting.value = false;
  }
};

const resetForm = () => {
  vipData.value = {
    vip_start_time: '',
    vip_end_time: '',
    vip_status:'Inactive'
  };
};
</script>

<style scoped>
.add-vip-form {
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