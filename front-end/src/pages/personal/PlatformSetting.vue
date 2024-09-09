<script lang="ts" setup>
  import { reactive } from 'vue';

  const groups = reactive([
    {
      title: '账户相关',
      children: [
        { checked: false, label: '特定位置附近新失物登记推送' },
        { checked: true, label: '关注失物状态改变弹窗' },
        { checked: true, label: '用户消息弹窗' },
        { checked: true, label: '陌生人消息' },
        { checked: true, label: '长辈模式' },
        { checked: true, label: '位置信息' },
      ],
    },
    {
      title: '应用相关',
      children: [
        { checked: true, label: '白天模式' },
        { checked: false, label: '短信接受消息' },
        { checked: true, label: '邮件接收消息' },
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
