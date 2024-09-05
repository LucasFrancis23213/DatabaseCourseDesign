<script lang="ts" setup>
  import { StepinHeaderAction } from 'stepin';
  import DayNightSwitch from '@/components/switch/DayNightSwitch.vue';
  import Fullscreen from '../fullscreen/Fullscreen.vue';
</script>
<template>
  <StepinHeaderAction>
    <DayNightSwitch />
  </StepinHeaderAction>
  <StepinHeaderAction>
    <Fullscreen class="-mx-xs -my-sm h-[56px] px-xs py-sm flex items-center" target=".stepin-layout" />
  </StepinHeaderAction>
</template>
<style scoped lang="less">
  .gitee-logo {
    width: 20px;
  }
  .action-item {
    font-size: 20px;
    height: 100%;
    margin: 0 -8px;
    padding: 0 4px;
    line-height: 40px;
    display: flex;
    align-items: center;

    &.setting {
      font-size: 18px;
    }

    &.notice {
      font-size: 18px;
    }
  }
</style>
