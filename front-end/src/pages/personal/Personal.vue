<template>
  <div class="personal">
    <div class="banner w-full rounded-xl p-base items-baseline">
      <div
        class="profile flex items-center justify-between p-base bg-container rounded-2xl absolute -bottom-16 left-6 shadow-lg"
      >
        <div class="info flex items-center">
          <img class="w-20 rounded-lg" src="@/assets/avatar/face-1.jpg" />
          <div class="flex flex-col justify-around ml-4">
            <div style="display: flex; align-items: center;">
              <span class="text-title text-xl font-bold">{{ userInfo.username }}</span>
              <EditFilled @click="edit" class="text-subtext hover:text-primary cursor-pointer" style="margin-left:10px" />
            </div>
          <span class="text-subtext font-semibold">ID: {{ userInfo.userId }}</span>
          <!-- 实名认证按钮 -->
          <a-button @click="isAuthModalVisible = true" type="primary" class="mt-2">实名认证</a-button>
</div>

        </div>
      </div>
    </div>
    <div class="mt-24 flex justify-evenly">
      <PlatformSetting class="flex-1" />
    </div>
    <a-divider class="my-10" />
  </div>
  <a-button @click="isDeleteModalVisible = true">注销</a-button>
  <!-- 实名认证模态框 -->
  <a-modal
    title="实名认证"
    v-model:visible="isAuthModalVisible"
    @ok="submitAuthentication"
    okText="提交"
    cancelText="取消"
  >
    <a-input placeholder="请输入您的真实姓名" v-model="realName" class="mb-2" />
    <a-input placeholder="请输入您的身份证号码" v-model="idCard" />
  </a-modal>
  <!-- 注销模态框 -->
  <a-modal
    title="确认注销账号"
    v-model:visible="isDeleteModalVisible"
    okText="确认"
    cancelText="取消"
    @Ok="deleteUser()" 
    @cancel="isDeleteModalVisible = false"
  >
  <template #default>
    是否注销账号: {{ accountStore.account.userName }}?
  </template>
  </a-modal>
  <a-modal
    v-model:visible="isEditModalVisible"
    title="编辑个人信息"
    ok-text="确认"
    cancel-text="取消"
    @ok="confirmEdit"
    @cancel="cancelEdit"
    style="max-width: 400px;"
  >
  <a-form :model="editRecord" :labelCol="{ span: 8 }" :wrapperCol="{ span: 12 }">
    <a-form-item label="用户ID" name="userID">
      <span>{{ editRecord.userID }}</span>
    </a-form-item>
    <a-form-item label="用户名" name="userName">
      <a-input v-model:value="editRecord.userName" :placeholder="userInfo.username"/>
    </a-form-item>
    <a-form-item label="绑定号码" name="contact">
      <a-input v-model:value="editRecord.contact" :placeholder="userInfo.contact" />
    </a-form-item>
  </a-form>
  </a-modal>
  <MyFind />
  <MySearch />
</template>

<script lang="ts" setup>
import { ref, computed } from 'vue';
import { message } from 'ant-design-vue';
import axios from 'axios';
import { useAccountStore } from '@/store/account';
import PlatformSetting from './PlatformSetting.vue';
import { useRouter } from 'vue-router';
import MyFind from './MyFind.vue';
import MySearch from './MySearch.vue';

const select = ref('overview');
const accountStore = useAccountStore();
const router = useRouter();
const realName = ref('');
const idCard = ref('');
const isAuthModalVisible = ref(false); // 控制实名认证模态框显示
const isDeleteModalVisible = ref(false); // 控制注销模态框显示
const isEditModalVisible = ref(false); //控制编辑信息模态框显示

// 访问 userInfo 数据
const userInfo = computed(() => ({
  username: accountStore.account.userName,
  userId: accountStore.account.userId,
  contact:accountStore.account.contact
}));

// 实名认证提交逻辑
const submitAuthentication = () => {
  console.log('Real Name:', realName.value);
  console.log('ID Card:', idCard.value);
  isAuthModalVisible.value = false; // 关闭模态框
};

// 删除用户方法
const deleteUser = () => {
  accountStore.deleteUser();
  router.push('/home');
};

const editRecord = ref({
  userID: '',
  userName: '',
  password:null,
  contact:''
});

function edit() {
  editRecord.value = {
    userID: userInfo.value.userId,
    userName: '',
    password:null,
    contact:''
  };
  isEditModalVisible.value = true;
}

function confirmEdit() {
  let url = `https://localhost:44343/api/UserManagement/UpdateUserInfo`;

  // 确保 editRecord 解构并传递到 API
  axios.put(url, editRecord.value)
    .then(() => {
      message.success('编辑成功！');
      accountStore.account.userName=editRecord.value.userName;
      accountStore.profile();
      isEditModalVisible.value = false;
    })
    .catch(error => {
      message.error('编辑失败，请重试');
      console.error('Error updating transaction:', error);
    });
}

function cancelEdit() {
  editRecord.value = {
    userID: '',
    userName: '',
    password:null,
    contact:''
  };
  isEditModalVisible.value = false;
}
</script>

<style lang="less" scoped>
  .personal {
    .banner {
      height: 240px;
      background-image: url('@/assets/personal-bg.png');
      background-position: 50% 10%;
      background-size: cover;
      position: relative;

      .profile {
        width: calc(~'100% - 48px');
      }

      :deep(.navi) {
        .ant-breadcrumb-link,
        .ant-breadcrumb-separator {
          color: rgba(255, 255, 255, 0.65);
        }

        & > span:last-child .ant-breadcrumb-link {
          @apply text-text-inverse;
        }
      }
    }
  }

  .read-the-docs {
  color: #888;
}
.second-text {
  color: #e60707;
}
</style>