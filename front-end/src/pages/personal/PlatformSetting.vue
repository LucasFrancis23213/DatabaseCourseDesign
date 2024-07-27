<script lang="ts" setup>
  import { reactive } from 'vue';

  const groups = reactive([
    {
      title: '账户通知',
      children: [
        { checked: false, label: '当有人关注我时发送邮件通知' },
        { checked: true, label: '当有人认领我发布的物品时发送邮件通知' },
        { checked: true, label: '当有人提到我时发送邮件通知' },
      ],
    },
    {
      title: '应用',
      children: [
        { checked: true, label: '有新的发布时发送通知' },
        { checked: false, label: '更新时发送通知' },
      ],
    },
  ]);
</script>
<template>
  <a-card
    :bordered="false"
    title="平台设置"
    class="shadow-lg platform-setting rounded-xl"
  >
    <div class="group" v-for="(group, i) in groups">
      <div class="text-xs font-medium text-subtext mb-6">
        {{ group.title }}
      </div>
      <div class="group-list">
        <div
          class="group-list-item flex items-center"
          v-for="item in group.children"
        >
          <a-switch v-model:checked="item.checked" class="mr-2" />
          {{ item.label }}
        </div>
      </div>
    </div>
  </a-card>
</template>
<style lang="less" scoped>
  .platform-setting {
    :deep(.ant-card-head) {
      @apply border-none;
      .ant-card-head-title {
        @apply font-semibold;
      }
    }
    :deep(.ant-card-body) {
      @apply pt-2;
    }
    .group {
      &:not(:first-child) {
        @apply mt-6;
      }
      &-list {
        &-item {
          &:not(:first-child) {
            @apply mt-6;
          }
        }
      }
    }
  }
</style>
