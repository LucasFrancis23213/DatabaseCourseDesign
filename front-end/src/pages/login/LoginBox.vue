<template>
  <ThemeProvider :color="{ middle: { 'bg-base': '#fff' }, primary: { DEFAULT: '#1896ff' } }">
      <div class="login-box rounded-sm relative">
        <!-- 使用 CloseCircleOutlined 图标作为关闭按钮 -->
        <CloseCircleOutlined class="close-btn absolute top-4 right-4 cursor-pointer text-2xl" @click="closeLogin" />
        <a-form
          :model="form"
          :wrapperCol="{ span: 24 }"
          @finish="login"
          class="login-form w-[400px] p-lg xl:w-[440px] xl:p-xl h-fit text-text"
        >
          <div class="third-platform">
            <div class="third-title mb-md text-lg">欢迎使用：</div>
          </div>
          <a-form-item :required="true" name="username">
            <a-input
              v-model:value="form.username"
              autocomplete="new-username"
              placeholder="请输入用户名:"
              class="login-input h-[40px]"
            />
          </a-form-item>
          <a-form-item :required="true" name="password">
            <a-input-password
              v-model:value="form.password"
              autocomplete="new-password"
              placeholder="请输入登录密码:"
              class="login-input h-[40px]"
              type="password"
            />
          </a-form-item>
          <router-link to='/signup'>
          没有账号？立即注册
          </router-link>
          <br><br>
          <a-button htmlType="submit" class="h-[40px] w-full" type="primary" :loading="loading"> 登录 </a-button>
          <a-divider></a-divider>
          <div class="terms">
            登录即代表您同意我们的
            <span class="font-bold">用户条款 </span>、<span class="font-bold"> 数据使用协议 </span>、以及
            <span class="font-bold">Cookie使用协议</span>。
          </div>
        </a-form>
      </div>
    </ThemeProvider>
  </template>
  <script lang="ts" setup>
    import { reactive, ref } from 'vue';
    import { useRouter } from 'vue-router';
    import { useAccountStore } from '@/store';
    import { ThemeProvider } from 'stepin';
    import { CloseCircleOutlined } from '@ant-design/icons-vue'; // 导入图标组件
    import { message } from 'ant-design-vue';

    export interface LoginFormProps {
      username: string;
      password: string;
    }
    const router = useRouter();
    const loading = ref(false);
  
    const form = reactive({
      username: undefined,
      password: undefined,
    });
  
    const emit = defineEmits<{
      (e: 'success', fields: LoginFormProps): void;
      (e: 'failure', reason: string, fields: LoginFormProps): void;
    }>();
  
    const accountStore = useAccountStore();
  
    async function login() {
      loading.value = true;
      const result = await accountStore.login(form.username, form.password);
      if (result.success) {
        message.success(result.message);
        emit('success', form);
      } else {
        message.error(result.message);
        emit('failure', result.message, form);
      }
      loading.value = false;
    }
    // 定义关闭登录界面的方法
    function closeLogin() {
      router.push('/');
    }
  </script>
  <style scoped lang="less">
    .login-box {
      position: relative; /* 确保可以绝对定位关闭按钮 */
    }
    .close-btn {
      position: absolute;
      top: 1rem; /* 距离顶部1rem */
      right: 1rem; /* 距离右侧1rem */
      font-size: 24px; /* 大小24px */
      color: #ccc; /* 颜色灰色 */
      &:hover {
        color: #f00; /* 鼠标悬停时变为红色 */
      }
    }
  </style>
