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
  import { useUnbounded } from '@/utils/useTheme';
  import { useAccountStore } from '@/store';

  useUnbounded();

  const { account } = useAccountStore();
</script>

<style scoped lang="less">
</style>
