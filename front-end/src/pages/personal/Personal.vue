<script lang="ts" setup>
  import { ref } from 'vue';
  import PlatformSetting from './PlatformSetting.vue';
  import ProfileInfo from './ProfileInfo.vue';
  import MySearch from './MySearch.vue';
  import MyFind from './MyFind.vue';

  const changeBackground = () => {

  }
  const changeAvatar = () => {

  }
  const changeName = () => {

  }
</script>
<template>
  <div class="personal">
    <div class="banner w-full rounded-xl p-base items-baseline">
      <div class="mt-0.5 text-text-inverse text-xl font-semibold">个人中心</div>
      <div
        class="profile flex items-center justify-between p-base bg-container rounded-2xl absolute -bottom-16 left-6 shadow-lg"
      >
        <div class="info flex items-center">
          <img class="w-20 rounded-lg" src="@/assets/avatar/face-1.jpg" />
          <div class="flex flex-col justify-around ml-4">
            <span class="text-title text-xl font-bold">小明</span>
            <span class="text-subtext font-semibold">个性签名</span>
          </div>
        </div>
        <a-button-group>
          <a-button @click="changeBackground">修改个人中心背景</a-button>
          <a-button @click="changeBackground">修改个性签名</a-button>
          <a-button @click="changeAvatar">修改头像</a-button>
          <a-button @click="changeName">修改名称</a-button>
        </a-button-group>
      </div>
    </div>
    <div class="mt-24 flex justify-evenly">
      <PlatformSetting class="flex-1" />
      <ProfileInfo class="flex-1 ml-lg" />
    </div>
    <a-divider class="my-10" />
  </div>
  
  <MySearch />
  <MyFind />

</template>
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
