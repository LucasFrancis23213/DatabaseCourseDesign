<script lang="ts" setup>
  import { reactive } from 'vue';
  import { StepinHeaderAction } from 'stepin';
  import Notice from '@/components/notice/Notice.vue';
  import DayNightSwitch from '@/components/switch/DayNightSwitch.vue';
  import { BellOutlined } from '@ant-design/icons-vue';
  import Fullscreen from '../fullscreen/Fullscreen.vue';

  defineEmits<{
    (e: 'showSetting'): void;
  }>();

  const noticeList = reactive([
    {
      title: '消息',
      list: [
        {
          title: '测试',
          content: 'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx',
          img: 'src/assets/avatar/face-1.jpg',
          time: 0,
        },
      ],
    },
    {
      title: '动态',
      list: [
        {
          title: '测试',
          content: 'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx',
          img: 'src/assets/avatar/face-1.jpg',
          time: 0,
        },
      ],
    },
    {
      title: '通知',
      list: [
        {
          title: '测试',
          content: 'xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx',
          img: 'src/assets/avatar/face-1.jpg',
          time: 0,
        },
      ],
    },
  ]);
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
