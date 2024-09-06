<script lang="ts" setup>
  import { GithubFilled } from '@ant-design/icons-vue';
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
        <span class="font-semibold">寻觅有道</span>
      </div>
      <p class="text-subtext slogan text-[1.5rem] xl:text-[1.75rem] font-extralight tracking-wide">
        基于vue的遗失物品寻找平台
      </p>
      <button
      @click="signIn"
      class="bg-black/50 text-white border border-black hover:bg-black cursor-pointer mt-lg shadow outline-none text-xl px-[64px] py-lg rounded-xl"
    >
      开始使用
      <br />
      <div class="text-base text-subtext"><GithubFilled /> @DatabaseCourseDesign</div>
    </button>
    
      <br />
      <br />
      <div class="inline-block slogan text-xl mr-lg">同济大学软件学院 2022级 数据库课程设计</div>
      <br />
    </div> 

  </div>
</template>
<style scoped lang="less">
  .home {
    min-height: max(100vh, 600px);
    margin-top: -78px;
  }
  .slogan {
    color: rgb(0, 0, 0); /* 设置颜色为黑色 */
  }
</style>
