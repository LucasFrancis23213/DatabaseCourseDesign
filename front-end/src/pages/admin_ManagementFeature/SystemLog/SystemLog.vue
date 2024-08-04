<script lang="ts" setup>
  import { ref } from 'vue';
  import AuthorTable from '../../table/AuthorTable.vue';
</script>
<template>
  <div style="display: flex; align-items: center;">
    <StepinHeaderAction style="flex-grow: 1; margin-right: 800px;">
        <a-input placeholder="开始搜索...">
            <template #prefix>
                <search-outlined />
            </template>
        </a-input>
    </StepinHeaderAction>
</div>
  <div class="table w-full">
    <AuthorTable />
  </div>
  <br>
  <div style="display: flex; justify-content: flex-end;">
    <a-button type="primary" style="margin-right: 20px;">
        下载日志
    </a-button>
</div>

</template>
