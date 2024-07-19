<template>
  <div class="personal">
    <div class="banner w-full rounded-xl p-base items-baseline">
      <a-breadcrumb class="navi">
        <a-breadcrumb-item class="text-text-inverse">Home</a-breadcrumb-item>
        <a-breadcrumb-item>Personal</a-breadcrumb-item>
      </a-breadcrumb>
      <div class="mt-0.5 text-text-inverse text-xl font-semibold">Overview</div>
      <div
        class="profile flex items-center justify-between p-base bg-container rounded-2xl absolute -bottom-16 left-6 shadow-lg"
      >
        <div class="info flex items-center">
          <img class="w-20 rounded-lg" src="@/assets/avatar/face-1.jpg" />
          <div class="flex flex-col justify-around ml-4">
          <span class="text-title text-xl font-bold">{{ userInfo.username }}</span>
          <span class="text-subtext font-semibold">ID: {{ userInfo.userId }}</span>
          <!-- 实名认证按钮 -->
          <a-button @click="isModalVisible = true" type="primary" class="mt-2">实名认证</a-button>
</div>

        </div>
        <a-radio-group v-model:value="select">
          <a-radio-button value="overview">OVERVIEW</a-radio-button>
          <a-radio-button value="teams">TEAMS</a-radio-button>
          <a-radio-button value="projects">PROJECTS</a-radio-button>
        </a-radio-group>
      </div>
    </div>
    <div class="mt-24 flex justify-evenly">
      <PlatformSetting class="flex-1" />
      <ProfileInfo class="flex-1 ml-lg" />
      <Conversation class="flex-1 ml-lg" />
    </div>
    <a-divider class="my-10" />
    <Projects class="mt-lg" />
  </div>
  <a-button @click="deleteUser()">注销</a-button>
  <!-- 实名认证模态框 -->
  <a-modal
    title="实名认证"
    v-model:visible="isModalVisible"
    @ok="submitAuthentication"
    okText="提交"
    cancelText="取消"
  >
    <a-input placeholder="请输入您的真实姓名" v-model="realName" class="mb-2" />
    <a-input placeholder="请输入您的身份证号码" v-model="idCard" />
  </a-modal>
</template>

<script lang="ts" setup>
import { ref, computed, onMounted } from 'vue';
import { useAccountStore } from '@/store/account'; // 确保正确导入你的 store
import Conversation from './Conversation.vue';
import PlatformSetting from './PlatformSetting.vue';
import ProfileInfo from './ProfileInfo.vue';
import Projects from './Projects.vue';
import { useRouter } from 'vue-router';

const select = ref('overview');
const accountStore = useAccountStore();
const router = useRouter();
const realName = ref('');
const idCard = ref('');
const isModalVisible = ref(false); // 控制模态框显示

// 当组件挂载完成后，从 Pinia store 加载用户信息
onMounted(async () => {
  if (accountStore.account.userName) { 
    await accountStore.profile();
  }
});

// 访问 userInfo 数据
const userInfo = computed(() => ({
  username: accountStore.account.userName,
  userId: accountStore.account.userId
}));

// 实名认证提交逻辑
const submitAuthentication = () => {
  console.log('Real Name:', realName.value);
  console.log('ID Card:', idCard.value);
  isModalVisible.value = false; // 关闭模态框
};

// 删除用户方法
const deleteUser = () => {
  accountStore.deleteUser();
  router.push('/home');
};
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
</style>
