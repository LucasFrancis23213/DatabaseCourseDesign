<script lang="ts" setup>
import { LogoutOutlined } from '@ant-design/icons-vue';
import { onMounted, ref } from 'vue';
import { ThemeProvider, alert } from 'stepin';
import { Popover } from 'ant-design-vue';

interface NavItem {
  title: string;
  link: string;
}

const navList = ref<NavItem[]>([
  {
    title: 'Developers',
    link: 'http://121.36.200.128:5600/Developers.html'
  },
  {
    title: 'About Us',
    link: 'https://github.com/LucasFrancis23213/DatabaseCourseDesign'
  },
]);

const handleNavClick = (nav: NavItem) => {
  if (nav.link.startsWith('http')) {
    window.open(nav.link, '_blank');
  } else {
    window.location.href = nav.link;
  }
};
</script>

<template>
  <ThemeProvider :color="{ middle: { 'bg-base': '#003f8c' }, primary: { DEFAULT: '#1896ff' } }" :autoAdapt="false">
    <div class="front-view flex flex-col">
      <div class="text-text flex-1">
        <div class="front-header flex items-baseline py-md px-xl">
          <router-link to="/home" class="text-xxl text-text hover:text-text">
            <img src="@/assets/logos.png" alt="Logo" />
            寻觅有道
          </router-link>
          <div
            style="width: calc(100% - 430px)"
            class="front-navigation mx-xl flex overflow-hidden items-center text-lg overflow-ellipsis whitespace-nowrap"
          >
            <div
              class="front-nav-item flex items-center cursor-pointer mx-base"
              v-for="nav in navList"
              :key="nav.title"
              @click="handleNavClick(nav)"
            >
              {{ nav.title }}
            </div>
          </div>
          <div>
            <router-link
              to="/login"
              class="h-[46px] border-transparent hover:text-text hover:border-transparent text-lg text-text"
            >
              <LogoutOutlined class="mr-xs" />
              Sign In
            </router-link>
            <a-button
              class="ml-md px-lg border-text hover:border-text hover:bg-text border-2 h-[46px] hover:text-bg-container"
              size="large"
              >Get Started</a-button
            >
          </div>
        </div>
        <div class="front-content px-xl">
          <router-view />
        </div>
      </div>
    </div>
  </ThemeProvider>
</template>

<style lang="less" scoped>
.front-view {
  .front-header {
    .front-nav-item {
      transition: color 0.3s;
      
      &:hover {
        color: #1896ff;
      }
    }
  }
  .front-content {
    min-height: calc(100vh - 78px);
  }
}
</style>