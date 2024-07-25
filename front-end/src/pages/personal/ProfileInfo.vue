<script lang="ts" setup>
import {
  EditFilled,
  FacebookOutlined,
  InstagramOutlined,
  TwitterOutlined,
} from '@ant-design/icons-vue';
import { reactive, ref } from 'vue';
import { usePersonalDescription } from '@/store/personal';

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

const open = ref<boolean>(false)
const loading = ref<boolean>(false)
const personalDescription = usePersonalDescription()
const newPersonalDescription = ref<string>("")
const editPersonalDescription = () => {
  open.value = true
}
const submitNewPersonalDescription = () => {
  personalDescription.updateDescription(newPersonalDescription.value)
    loading.value = true
    setTimeout(() => {
          location.reload();
          loading.value = false;
          open.value = false;
        }, 1000);
}

</script>
<template>
  <a-card title="个人简介" class="profile-info rounded-xl shadow-lg" :bordered="false">
    <a-modal v-model:visible="open" title="更改个人简介" >
      <template #footer></template>
      <a-form :labelCol="{ span: 5 }" :wrapperCol="{ span: 16 }" >
        <a-form-item label="新个人简介" name="newDescription" has-feedback >
          <a-input v-model:value="newPersonalDescription" :maxlength="200" />
        </a-form-item>
        <a-form-item :wrapper-col="{ offset: 10, span: 16 }">
          <a-button type="primary" html-type="submit" @click="submitNewPersonalDescription" :loading="loading">更新</a-button>
        </a-form-item>
      </a-form>
    </a-modal>

    <template #extra>
      <EditFilled @click="editPersonalDescription" class="text-subtext hover:text-primary cursor-pointer" />
    </template>
    <div class="description">
      {{ personalDescription.description }}
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
