<script lang="ts" setup></script>
<template>
  <div class="page-footer">
    <div class="links">
    </div>
  </div>
</template>

<style scoped lang="less">
  .page-footer {
    text-align: center;
    @apply text-gray-400;

    .links {
      display: flex;
      justify-content: center;

      .link {
        @apply hover:text-gray-400 pl-4 pr-4;
      }
    }

    .copyright {
      margin-top: 8px;

      .icon-copyright {
        margin: 0;
      }
    }
  }
</style>
