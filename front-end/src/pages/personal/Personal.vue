<script lang="ts" setup>
  import { ref } from 'vue';
  import PlatformSetting from './PlatformSetting.vue';
  import ProfileInfo from './ProfileInfo.vue';
  import MySearch from './MySearch.vue';
  import MyFind from './MyFind.vue';
  import { usePersonalSignature } from '@/store/personal';

  const personalSignature = usePersonalSignature()
  const open = ref<boolean>(false)
  const loading = ref<boolean>(false)
  const newSignature = ref<string>("")

  const changeSignature = () => {
    open.value = true
  }
  const submitNewSignature = () => {
    personalSignature.updateSignature(newSignature.value)
    loading.value = true
    setTimeout(() => {
          location.reload();
          loading.value = false;
          open.value = false;
        }, 1000);
  }
</script>
<template>
  <a-modal v-model:visible="open" title="更改个性签名" >
    <template #footer></template>
    <a-form :labelCol="{ span: 5 }" :wrapperCol="{ span: 16 }" >
      <a-form-item label="新签名" name="newSignature" has-feedback >
        <a-input v-model:value="newSignature" :maxlength="50" />
      </a-form-item>
      <a-form-item :wrapper-col="{ offset: 10, span: 16 }">
        <a-button type="primary" html-type="submit" @click="submitNewSignature" :loading="loading">更新</a-button>
      </a-form-item>
    </a-form>
  </a-modal>

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
            <span class="text-subtext font-semibold">{{ personalSignature.signature }}</span>
          </div>
        </div>
        <a-button @click="changeSignature" type="primary">修改个性签名</a-button>
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
