<script lang="ts" setup>
  import { useRouter } from 'vue-router';

  const router = useRouter();
  function signIn() {
    router.push('/login');
  }
</script>
<template>
  <div class="home text-center flex flex-col justify-center">
    <div class="transform w-full">
      <div class="tracking-wide slogan text-[3.5rem] xl:text-[5.25rem] font-extralight">
        <span class="font-semibold"></span>
      </div>
      <button
        @click="signIn"
        class="bg-primary-300 hover:bg-primary-400 cursor-pointer mt-lg shadow border-0 outline-none text-xl px-[64px] py-lg rounded-sm"
      >
        Sign In and Start
        <br />
      </button>
      <br />
    </div>
  </div>
</template>
<style scoped lang="less">
  .home {
    min-height: max(100vh, 600px);
    margin-top: -78px;
  }
</style>
