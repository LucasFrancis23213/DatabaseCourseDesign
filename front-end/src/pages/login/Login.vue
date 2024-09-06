<template>
  <div class="login flex items-center justify-center">
    <login-box class="shadow-lg" @success="onLoginSuccess" />
  </div>
</template>
<script lang="ts" setup>
  import LoginBox from './LoginBox.vue';
  import { useRouter } from 'vue-router';
  import { useAccountStore } from '@/store/account';

  const router = useRouter();
  const {permissions} = useAccountStore();
  function onLoginSuccess() {
  router.push('/Personal');}
</script>

<style scoped lang="less">
  .login {
    min-height: max(100vh, 720px);
    margin-top: -78px;
  }
</style>
