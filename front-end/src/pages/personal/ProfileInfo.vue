<script lang="ts" setup>
import EditableCell from '@/components/editable-cell';
import {
  EditFilled,
  FacebookOutlined,
  InstagramOutlined,
  TwitterOutlined,
} from '@ant-design/icons-vue';
import { reactive, ref } from 'vue';

const profiles = reactive([
  {
    label: '名称',
    content: 'Li Zhi',
  },
  {
    label: '联系',
    content: '13678988900',
  },
  {
    label: '邮箱',
    content: 'lizhi@firfox.com',
  },
  {
    label: '地址',
    content: 'shenzheng.CN',
  },
]);

const edit = ref(false)
</script>
<template>
  <a-card title="个人简介" class="profile-info rounded-xl shadow-lg" :bordered="false">
    <template #extra>
      <EditFilled @click="edit = true" class="text-subtext hover:text-primary cursor-pointer" />
    </template>
    <div class="description">
      <EditableCell :options="{ rows: 4 }" v-model:edit="edit" type="textarea" value="Hi, I’m Alec Thompson, Decisions: If you can’t decide, the answer is no.
      If two equally difficult paths, choose the one more painful in the short
      term (pain avoidance is creating an illusion of equality).">
      </EditableCell>
    </div>
    <a-divider />
    <a-descriptions class="profile-list mt-3 font-medium" :column="1">
      <a-descriptions-item :label="item.label" v-for="item in profiles">
        {{ item.content }}
      </a-descriptions-item>
      <a-descriptions-item label="社交">
        <TwitterOutlined class="text-blue-400" />
        <FacebookOutlined class="text-blue-800" />
        <InstagramOutlined class="text-red-500" />
      </a-descriptions-item>
    </a-descriptions>
  </a-card>
</template>
<style lang="less" scoped>
.profile-info {
  :deep(.ant-card) {
    &-head {
      @apply border-none;

      &-title {
        @apply font-semibold;
      }
    }

    &-body {
      @apply pt-1;
    }
  }

  :deep(.ant-descriptions) {
    &-row:last-child>td {
      padding-bottom: 0;
    }

    &-item {
      &-content {
        @apply items-center;

        .anticon {
          @apply text-base;

          &:not(:first-child) {
            @apply ml-2;
          }
        }
      }
    }
  }
}
</style>
