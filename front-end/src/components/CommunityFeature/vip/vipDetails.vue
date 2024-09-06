<template>
  <div class="vip-details">
    <div class="vip-info">
      <div class="vip-text">
        <p><strong>用户ID:</strong> {{ props.user_id }}</p>
        <p><strong>注册时间:</strong> {{ formatDate(vipInfo.vip_start_time) }}</p>
        <p><strong>VIP 到期时间:</strong> {{ formatDate(vipInfo.vip_end_time) }}</p>
        <p><strong>VIP状态:</strong> {{ vipInfo.vip_status }}</p>


      </div>
    </div>
    <div class="vip-actions">
      <button @click="editVip" class="action-button edit">修改VIP信息</button>
      <button @click="deleteVip" class="action-button delete">删除VIP用户</button>
    </div>
    <div v-if="isEditing" class="edit-form">
      <div class="form-group">
        <label for="end_time">到期时间:</label>
        <input id="end_time" type="datetime-local" v-model="editedVip.vip_end_time" placeholder="请选择到期时间">
      </div>
      <div class="form-group">
        <label for="vip_status">vip状态</label>
        <select id="vip_status" v-model="editedVip.vip_status" required>
          <option value="Active">有效</option>
          <option value="Inactive">无效</option>
          <option value="Cancelled">取消</option>
        </select>
      </div>


      <button @click="saveEdit" class="action-button save">保存修改</button>
      <button @click="cancelEdit" class="action-button cancel">取消</button>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from "axios";
import {message} from "ant-design-vue";

axios.defaults.baseURL = import.meta.env.VITE_API_URL;

const props = defineProps(['user_id']);
const emit = defineEmits(['update', 'delete']);

const isEditing = ref(false);
const vipInfo = ref(null);
const editedVip = ref({...vipInfo.value});
const showOrderDetails = ref(false);

function formatDate(dateString) {
  const options = {year: 'numeric', month: 'long', day: 'numeric', hour: '2-digit', minute: '2-digit'};
  return new Date(dateString).toLocaleDateString('zh-CN', options);
}

function toggleOrderDetails() {
  showOrderDetails.value = !showOrderDetails.value;
}

function getOrderStatus(status) {
  const statuses = {
    'completed': '已完成',
    'pending': '待处理',
    'cancelled': '已取消'
  };
  return statuses[status] || status;
}

function editVip() {
  isEditing.value = true;
  editedVip.value = {...vipInfo.value};
}

function validateTime() {
  return editedVip.value.vip_end_time > editedVip.value.vip_start_time;
}

async function saveEdit() {
  try {
    const changedParams = {};
    for (const key in editedVip.value) {
      if (editedVip.value[key] !== vipInfo.value[key]) {
        changedParams[key] = editedVip.value[key];
      }
    }

    if (Object.keys(changedParams).length === 0) {
      alert('没有修改任何内容');
      isEditing.value = false;
      return;
    }
    if (!validateTime()) {
      message.error('vip结束时间必须晚于开始时间');
      return;
    }
    const response = await axios.put('/api/vip/UpdateVipMember', {
      user_id: props.user_id,
      vip_member_id: vipInfo.value.vip_member_id,
      ...changedParams
    });

    if (response.status === 200) {
      Object.assign(vipInfo.value, changedParams);
      isEditing.value = false;
      alert('VIP信息更新成功');
    } else {
      throw new Error('Failed to update VIP info');
    }
  } catch (error) {
    console.error('Error updating VIP info:', error);
    alert('更新VIP信息失败，请稍后重试');
  }
}

function cancelEdit() {
  isEditing.value = false;
}

async function deleteVip() {
  if (confirm('确定要删除这个VIP用户吗？')) {
    try {
      const response = await axios.delete('/api/vip/DeleteVipMember', {
        data: {
          user_id: props.user_id,
          vip_member_id: vipInfo.value.vip_member_id
        }
      });

      if (response.status === 200) {
        alert('VIP用户已成功删除');
        emit('delete', props.user_id);
      } else {
        throw new Error('Failed to delete VIP user');
      }
    } catch (error) {
      console.error('Error deleting VIP user:', error);
      alert('删除VIP用户失败，请稍后重试');
    }
  }
}

async function getVipInfo() {
  try {
    const res = await axios.post('api/vip/GetVIPstatus', {
      user_id: props.user_id
    });
    vipInfo.value = res.data.vip_info;
    console.log(vipInfo.value)
  } catch (e) {
    console.error(e);
    alert(`获取VIP详情失败，错误原因：${e}`);
  }
}

onMounted(() => {
  getVipInfo();
})
</script>

<style scoped>
.vip-details {
  width: 100%;
  margin: 20px auto;
  background-color: #ffffff;
  padding: 30px;
  border-radius: 12px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.vip-title {
  color: #2c3e50;
  font-size: 2em;
  margin-bottom: 20px;
  border-bottom: 3px solid #3498db;
  padding-bottom: 15px;
  text-align: center;
}

.vip-info {
  display: flex;
  justify-content: space-between;
  margin-bottom: 30px;
}

.vip-text {
  flex: 1;
}

.vip-text p {
  margin: 10px 0;
  font-size: 1.1em;
  color: #34495e;
}

.vip-text strong {
  color: #2c3e50;
  font-weight: 600;
}

.vip-actions {
  display: flex;
  justify-content: space-between;
  margin-top: 20px;
}

.action-button {
  padding: 10px 20px;
  border: none;
  border-radius: 6px;
  font-size: 1em;
  cursor: pointer;
  transition: all 0.3s ease;
}

.edit {
  background-color: #3498db;
  color: white;
}

.delete {
  background-color: #e74c3c;
  color: white;
}

.edit:hover, .delete:hover {
  opacity: 0.8;
}

.edit-form {
  margin-top: 30px;
  background-color: #f8f9fa;
  padding: 20px;
  border-radius: 8px;
}

.form-group {
  margin-bottom: 20px;
}

.form-group label {
  display: block;
  margin-bottom: 5px;
  color: #2c3e50;
  font-weight: 600;
}

.form-group input,
.form-group select {
  width: 100%;
  padding: 10px;
  border: 1px solid #bdc3c7;
  border-radius: 4px;
  font-size: 1em;
}

.save, .cancel {
  margin-top: 20px;
  margin-right: 10px;
}

.save {
  background-color: #2ecc71;
  color: white;
}

.cancel {
  background-color: #95a5a6;
  color: white;
}
</style>

<!--        <button @click="toggleOrderDetails" class="details-button">-->
<!--          {{ showOrderDetails ? '隐藏订单详情' : '查看订单详情' }}-->
<!--        </button>-->
<!--        <div v-if="showOrderDetails" class="order-details">-->
<!--          <div class="table-container">-->
<!--            <table class="styled-table">-->
<!--              <thead>-->
<!--                <tr>-->
<!--                  <th>订单ID</th>-->
<!--                  <th>订单时间</th>-->
<!--                  <th>订单金额</th>-->
<!--                  <th>订单状态</th>-->
<!--                </tr>-->
<!--              </thead>-->
<!--              <tbody>-->
<!--                <tr v-for="order in vipInfo.order_history" :key="order.order_id">-->
<!--                  <td>{{ order.order_id }}</td>-->
<!--                  <td>{{ formatDate(order.order_time) }}</td>-->
<!--                  <td>{{ order.amount }} 元</td>-->
<!--                  <td>{{ getOrderStatus(order.status) }}</td>-->
<!--                </tr>-->
<!--              </tbody>-->
<!--            </table>-->
<!--          </div>-->
<!--        </div>-->