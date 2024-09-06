<template>
  <div class="workplace grid grid-rows-none gap-4 mt-xxs">
    <div class="bg-container p-base rounded-b-lg rounded-tr-lg pt-8 flex items-end justify-between">
      <div class="flex items-center">
        <img src="@/assets/avatar/face-1.jpg" class="w-16 h-16 rounded-full" />
        <div class="ml-base">
          <div class="text-title font-bold text-lg">{{ account.userName }}</div>
          <div class="text-subtext font-bold text-sm">管理员</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
  import { reactive } from 'vue';
  import MiniStatisticCard from '@/components/statistic/MiniStatisticCard.vue';
  import ActiveUsers from './ActiveUsers.vue';
  import SalesOverview from './SalesOverview.vue';
  import Projects from './Projects.vue';
  import OrderHistory from './OrderHistory.vue';
  import AboutUs from './AboutUs.vue';
  import { useUnbounded } from '@/utils/useTheme';
  import { useAccountStore } from '@/store';

  useUnbounded();

  const { account } = useAccountStore();

  const statisticList = reactive([
    {
      title: '今日销售额',
      value: '￥53,000',
      change: '+31%',
      icon: 'dollar-circle-filled',
      iconClass: 'text-blue-100',
    },
    {
      title: '今日新增用户',
      value: '138',
      change: '+23%',
      icon: 'usergroup-add-outlined',
      iconClass: 'text-purple-100',
    },
    {
      title: '今日新增客户',
      value: '￥5000',
      change: '30%',
      icon: 'heart-filled',
      iconClass: 'text-primary-100',
    },
    {
      title: '今日订单量',
      value: '3200',
      change: '-11%',
      icon: 'shopping-filled',
      iconClass: 'text-green-100',
    },
  ]);
</script>

<style scoped lang="less">
  .workplace {
  }
</style>
