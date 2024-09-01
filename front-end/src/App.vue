<template>
  <a-config-provider :getPopupContainer="getPopupContainer">
    <ThemeProvider is-root v-bind="themeConfig" :apply-style="false">
      <stepin-view
        system-name="寻觅有道"
        logo-src="@/assets/logos.png"
        :class="`${contentClass}`"
        :user="user"
        :navMode="navigation"
        :useTabs="useTabs"
        :themeList="themeList"
        v-model:show-setting="showSetting"
        v-model:theme="theme"
        @themeSelect="configTheme"
      >
        <template #headerActions>
          <HeaderActions @showSetting="showSetting = true" />
        </template>
        <template #pageFooter>
          <PageFooter />
        </template>
        <template #themeEditorTab>
          <a-tab-pane tab="其它" key="other">
            <Setting />
          </a-tab-pane>
        </template>
      </stepin-view>
    </ThemeProvider>
  </a-config-provider>
</template>

<script lang="ts" setup>
  import { reactive, ref, onMounted, onBeforeUnmount } from 'vue';
  import { useRouter } from 'vue-router';
  import { useAccountStore, useMenuStore, useSettingStore, storeToRefs } from '@/store';
  import avatar from '@/assets/avatar.png';
  import { PageFooter, HeaderActions } from '@/components/layout';
  import Setting from './components/setting';
  import { configTheme, themeList } from '@/theme';
  import { ThemeProvider } from 'stepin';
  import { useAuthStore } from '@/plugins';
  import { computed } from 'vue';

  const { logout, account, permissions, closeApp} = useAccountStore();
  const showSetting = ref(false);
  const router = useRouter();

  const { navigation, useTabs, theme, contentClass } = storeToRefs(useSettingStore());
  const themeConfig = computed(() => themeList.find((item) => item.key === theme.value)?.config ?? {});

  const user = reactive({
    get name() {
      return account.userName;
    },
    avatar: avatar,
    menuList: [
      { title: '个人中心', key: 'personal', icon: 'UserOutlined', onClick: () => router.push('./Personal') },
      { title: '设置', key: 'setting', icon: 'SettingOutlined', onClick: () => (showSetting.value = true) },
      { type: 'divider' },
      {
        title: '退出登录',
        key: 'logout',
        icon: 'LogoutOutlined',
        onClick: () => logout().then(() => router.push('/home')),
      },
    ],
  });

  if(user.name!==''){
    useMenuStore().getMenuList();
    useAuthStore().setAuthorities(permissions);
  }

  function getPopupContainer() {
    return document.querySelector('.stepin-layout');
  }
</script>

<style lang="less">
  .stepin-view {
    ::-webkit-scrollbar {
      width: 4px;
      height: 4px;
      border-radius: 4px;
      background-color: theme('colors.primary.500');
    }

    ::-webkit-scrollbar-thumb {
      border-radius: 4px;
      background-color: theme('colors.primary.400');

      &:hover {
        background-color: theme('colors.primary.500');
      }
    }

    ::-webkit-scrollbar-track {
      box-shadow: inset 0 0 1px rgba(0, 0, 0, 0);
      border-radius: 4px;
      background: theme('backgroundColor.layout');
    }
  }

  html {
    height: 100vh;
    overflow-y: hidden;
  }

  body {
    margin: 0;
    height: 100vh;
    overflow-y: hidden;
  }
  .stepin-img-checkbox {
    @apply transition-transform;
    &:hover {
      @apply scale-105 ~"-translate-y-[2px]";
    }
    img {
      @apply shadow-low rounded-md transition-transform;
    }
  }
</style>