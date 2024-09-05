<script lang="ts" setup>
  import { LogoutOutlined } from '@ant-design/icons-vue';
  import { ThemeProvider } from 'stepin';
</script>
<template>
  <ThemeProvider :color="{ middle: { 'bg-base': '#003f8c' }, primary: { DEFAULT: '#1896ff' } }" :autoAdapt="false">
    <div class="front-view flex flex-col">
      <div class="text-text flex-1">
        <div class="front-header flex items-baseline py-md px-xl">
          <div
            style="width: calc(100% - 430px)"
            class="front-navigation mx-xl flex overflow-hidden items-center text-lg overflow-ellipsis whitespace-nowrap"
          >
            <div
              :class="`front-nav-item flex items-center cursor-pointer mx-base}`"
            >
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
        &.with-list .front-nav-item-content {
          &:after {
            content: '';
            @apply ~"h-[8px]" ~"w-[8px]" transition-transform ml-2 inline-block border-text border-l-0 border-t-0 border-r-2 border-b-2 border-solid ~"rotate-[-135deg]" translate-y-1/4;
          }
          &:hover {
            &:after {
              @apply ~"rotate-[45deg]" translate-y-0;
            }
          }
        }
      }
    }
    .front-content {
      min-height: calc(100vh - 78px);
    }
  }
</style>
