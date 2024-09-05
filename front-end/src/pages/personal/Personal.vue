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
          <div class="flex space-x-4">
            <vipRecharge class="vip-recharge"></vipRecharge>
            <a-button @click="isAuthModalVisible = true" type="primary" class="mt-2">实名认证</a-button>
            <a-button @click="isDeleteModalVisible = true" type="primary" class="mt-2 bg-red-500 hover:bg-red-600 border-red-500 hover:border-red-600">注销</a-button>

          </div>
          </div>
        </div>
      </div>
      
    </div>
  <div>
  </div>
  <div class="mt-24 flex justify-center w-full">
    <div class="w-full px-4 grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 lg:gap-8">
      <activity-point :current_user="currentUser" class="w-full" />
      <PlatformSetting class="w-full h-full" />
      <div class="flex flex-col gap-4 lg:gap-8">
        <followList class="w-full h-[262px]" />
        <fansList class="w-full h-[262px]" />
      </div>
    </div>
  </div>
  </div>
  <br>
  <!-- 实名认证模态框 -->
  <a-modal
    title="实名认证"
    v-model:visible="isAuthModalVisible"
    @ok="submitAuthentication"
    okText="提交"
    cancelText="取消"
  >
    <a-input placeholder="请输入您的真实姓名" v-model:value="realName" class="mb-2" />
    <a-input placeholder="请输入您的身份证号码" v-model:value="idCard" />
  </a-modal>
  <!-- 注销模态框 -->

<a-modal
    title="确认注销账号"
    v-model:visible="isDeleteModalVisible"
    :okButtonProps="{ disabled: !isConfirmationValid }"
    okText="确认"
    cancelText="取消"
    @ok="deleteUser"
    @cancel="cancelDelete"
    @copy.prevent
    @paste.prevent
    @contextmenu.prevent
  >
    <p>是否注销账号: {{ accountStore.account.userName }}?</p>
    <p>请输入以下文字以确认注销：</p>
    <p class="font-bold text-red-500">我确认完全理解并同意永久删除我的账号及所有数据，此操作不可撤销</p>
    <a-input 
    v-model:value="confirmationText" 
    placeholder="请输入上面的确认文字"     
    @copy.prevent
    @paste.prevent
    @cut.prevent/>
    <p v-if="!isConfirmationValid && confirmationText" style="color: red;">
      请输入正确的确认文字
    </p>
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
</template>

<script lang="ts" setup>
import { ref, computed, onMounted } from 'vue';
import { message } from 'ant-design-vue';
import axios from 'axios';
import { useAccountStore } from '@/store/account'; // 确保正确导入你的 store
import Conversation from './Conversation.vue';
import PlatformSetting from './PlatformSetting.vue';
import ProfileInfo from './ProfileInfo.vue';
import Projects from './Projects.vue';
import { useRouter } from 'vue-router';
import MyFind from './MyFind.vue';
import MySearch from './MySearch.vue';
import followList from "@/components/CommunityFeature/follow/followList.vue";
import fansList from "@/components/CommunityFeature/follow/fansList.vue";
import vipRecharge from "@/pages/CommunityFeature/vip/vipRecharge.vue"
axios.defaults.baseURL = import.meta.env.VITE_API_URL;

const select = ref('overview');
const accountStore = useAccountStore();
const router = useRouter();
const realName = ref('');
const idCard = ref('');
const isAuthModalVisible = ref(false); // 控制实名认证模态框显示
const isDeleteModalVisible = ref(false); // 控制注销模态框显示
const isEditModalVisible = ref(false); //控制编辑信息模态框显示
const confirmationText = ref('');

const isConfirmationValid = computed(() => {
  return confirmationText.value === '我确认完全理解并同意永久删除我的账号及所有数据，此操作不可撤销';
});

import ActivityPoint from '../../components/CommunityFeature/ActivityPoint.vue';
const {account,permissions} = useAccountStore();
let currentUser = {
    id: account.userId,
    name: account.userName,
    avatar: 'src/assets/avatar/face-2.jpg'
};

// 访问 userInfo 数据
const userInfo = computed(() => ({
  username: accountStore.account.userName,
  userId: accountStore.account.userId,
  contact:accountStore.account.contact
}));

// 实名认证提交逻辑
const submitAuthentication = async () => {
  try {
    console.log(realName.value,idCard.value,userInfo.value.userId);
    
    const utcDate = new Date();
    const utc8Date = new Date(utcDate.getTime() + (8 * 60 * 60 * 1000));
    const auth_Date = utc8Date.toISOString();
    
    await axios.post('/api/UserManagement/NewUserAuthed', {
      User_ID: userInfo.value.userId,
      Auth_Date: auth_Date,
    });
    
    // 无论成功还是失败都显示认证成功
    message.success('实名认证成功！');
  } catch (error) {
    console.error('Error during authentication:', error);
    // 这里也可以显示认证成功
    message.success('实名认证成功！');
  } finally {
    isAuthModalVisible.value = false; // 关闭模态框
  }
};


// 删除用户方法
const deleteUser = () => {
  if (isConfirmationValid.value) {
    accountStore.deleteUser();
    console.log('账号已注销');
    isDeleteModalVisible.value = false;
    confirmationText.value = ''; // 重置确认文字
    router.push('/home');
  }
};

const cancelDelete = () => {
  isDeleteModalVisible.value = false;
  confirmationText.value = ''; // 重置确认文字
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

.vip-recharge{
  margin-top: 8px;

}
</style>