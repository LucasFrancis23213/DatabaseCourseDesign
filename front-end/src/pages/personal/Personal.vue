<template>
  <div class="personal">
    <div class="banner w-full rounded-xl p-base items-baseline">
      <div
        class="profile flex items-center justify-between p-base bg-container rounded-2xl absolute -bottom-16 left-6 shadow-lg"
      >
        <div class="info flex items-center">
          <img class="w-20 rounded-lg" :src="account.avatar" />
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
    <!-- 头像选择 -->
    <a-form-item label="头像">
      <div style="display: flex; align-items: center;">
    <img :src="editRecord.Avatar" style="cursor: pointer; width: 80px; height: 80px; border-radius: 50%;">
    <EditFilled @click="isAvatarModalVisible = true" class="text-subtext hover:text-primary cursor-pointer" style="margin-left:10px" />
  </div>
    </a-form-item>
    <a-form-item label="用户ID" name="userID">
      <span>{{ accountStore.account.userId }}</span>
    </a-form-item>
    <!-- 用户名输入 -->
    <a-form-item label="用户名" name="userName">
      <a-input v-model:value="editRecord.userName" :placeholder="userInfo.username"/>
    </a-form-item>

    <!-- 绑定号码输入 -->
    <a-form-item label="绑定号码" name="contact">
      <a-input v-model:value="editRecord.contact" :placeholder="userInfo.contact" />
    </a-form-item>
  </a-form>
</a-modal>

<!-- 头像选择模态框 -->
<a-modal
  v-model:visible="isAvatarModalVisible"
  title="选择头像"
  footer=""
  style="max-width: 200px; position: absolute; top: 10%; right: 20%;" 
>
  <div style="display: flex; flex-wrap: wrap; justify-content: space-around;">
    <div v-for="(avatar, index) in avatars" :key="index" style="margin: 10px;">
      <img 
        :src="avatar" 
        @click="editRecord.Avatar=`${baseURL}${index+1}.jpg`,isAvatarModalVisible=false" 
        style="width: 50px; height: 50px; cursor: pointer; border-radius: 50%;">
    </div>
  </div>
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
const isAvatarModalVisible = ref(false); //控制头像信息模态框显示
const confirmationText = ref('');
const baseURL = 'http://121.36.200.128:5600/Avatars/default_avatar';
const avatars = Array.from({ length: 10 }, (v, i) => `${baseURL}${i + 1}.jpg`);

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
// 校验身份证号码是否有效
function isValidIDCard(idCard) {
  const weights = [7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2];
  const checkCodes = ['1', '0', 'X', '9', '8', '7', '6', '5', '4', '3', '2'];
  //const idCardValue = idCard.value.trim(); // 获取去掉首尾空格的身份证号

  if (idCard.length !== 18) {
    return false;
  }

  // 校验前17位是否为数字
  const firstSeventeenDigits = idCard.slice(0, 17);

// 遍历前17位，检查每一位是否为数字
for (let i = 0; i < firstSeventeenDigits.length; i++) {
  const char = firstSeventeenDigits[i];

  // 使用 isNaN 来判断字符是否为数字，如果不是数字，返回 false
  if (isNaN(char)) {
    return false;
  }
}
  // 计算校验码
  let sum = 0;
  for (let i = 0; i < 17; i++) {
    sum += firstSeventeenDigits[i] * weights[i];
  }
  const mod = sum % 11;
  const checkCode = checkCodes[mod];

  // 校验第18位
  return idCard[17].toUpperCase() === checkCode;
}

const submitAuthentication = async () => {
  const idCardValue = idCard.value.trim();

  if (!isValidIDCard(idCardValue)) {
    message.error('身份证号码格式不正确，请检查后重试。');
    return;
  }

  try {
    await axios.post('/api/UserManagement/NewUserAuthed', {
      RealName: realName.value,
      IDCard: idCardValue,
    });
    message.success('实名认证成功！');
  } catch (error) {
    message.success('实名认证成功！');
  } finally {
    isAuthModalVisible.value = false;
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
  userID: account.userId,
  userName: null,
  password:null,
  contact:null,
  Avatar:account.avatar
});

function edit() {
  editRecord.value=({
  userID: account.userId,
  userName: null,
  password:null,
  contact:null,
  Avatar:account.avatar
});
  isEditModalVisible.value = true;
}

function confirmEdit() {
  let url = axios.defaults.baseURL + 'api/UserManagement/UpdateUserInfo';
  console.log(editRecord.value);

  // 确保 editRecord 解构并传递到 API
  axios.put(url, editRecord.value)
    .then(() => {
      message.success('编辑成功！');
      if(editRecord.value.userName!=null){
        accountStore.account.userName=editRecord.value.userName;
      }
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
    userID: accountStore.account.userId,
    userName: null,
    password:null,
    contact:null,
    Avatar:null
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