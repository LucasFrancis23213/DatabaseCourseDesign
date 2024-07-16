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
</template>
<script lang="ts" setup>
import { ref,computed,onMounted } from 'vue';
import { useAccountStore } from '@/store/account'; // 确保正确导入你的 store
import Conversation from './Conversation.vue';
import PlatformSetting from './PlatformSetting.vue';
import ProfileInfo from './ProfileInfo.vue';
import Projects from './Projects.vue';
import { Modal } from 'ant-design-vue';
import { useRouter } from 'vue-router';

const select = ref('overview');
const accountStore = useAccountStore();
const router = useRouter();

// 当组件挂载完成后，从 Pinia store 加载用户信息
onMounted(async () => {
  if (!!accountStore.account.userName) { 
    await accountStore.profile();
  }
});

// 访问 userInfo 数据
const userInfo = computed(() => ({
  username: accountStore.account.userName,
  userId: accountStore.account.userId
}));

const deleteUser = () => {
      Modal.confirm({
        title: '确认注销账号',
        content: `是否注销账号：${accountStore.account.userName}?`,
        okText: '确认',
        cancelText: '注销', 
        onOk() {
          accountStore.deleteUser();
          router.push('/home');
        },
        onCancel() {
          console.log('Cancel delete operation');
        }
      });
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
