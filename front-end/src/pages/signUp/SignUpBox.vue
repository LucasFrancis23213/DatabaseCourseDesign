<template>
  <ThemeProvider :color="{ middle: { 'bg-base': '#fff' }, primary: { DEFAULT: '#1896ff' } }">
      <div class="login-box rounded-sm relative">
        <!-- 使用 CloseCircleOutlined 图标作为关闭按钮 -->
        <CloseCircleOutlined class="close-btn absolute top-4 right-4 cursor-pointer text-2xl" @click="closeLogin" />
        <br>
        <a-form
          :model="form"
          :wrapperCol="{ span: 24 }"
          @finish="signUp"
          class="login-form w-[400px] p-lg xl:w-[440px] xl:p-xl h-fit text-text"
        >
          <a-form-item :required="true" name="username">
            <a-input
              v-model:value="form.username"
              autocomplete="new-username"
              placeholder="请输入用户名:"
              class="login-input h-[40px]"
            />
          </a-form-item>
          <a-form-item :required="true" name="password">
            <a-input
              v-model:value="form.password"
              autocomplete="new-password"
              placeholder="请输入自定义密码:"
              class="login-input h-[40px]"
              type="password"
            />
          </a-form-item>
          <a-form-item :required="true" name="contact">
            <a-input
              v-model:value="form.contact"
              autocomplete="new-contact"
              placeholder="请输入联系方式（邮箱或手机号）:"
              class="login-input h-[40px]"
            />
          </a-form-item>
          <form>
          <div class="terms">
            <label>
              <input type="checkbox" id="agree" name="agree" required>
              同意我们的
              <span class="font-bold">用户条款</span>、
              <span class="font-bold">数据使用协议</span>、以及
              <span class="font-bold">Cookie使用协议</span>。
            </label>
          </div>
          <a-divider></a-divider>
          <a-button htmlType="submit" class="h-[40px] w-full" type="primary" :loading="loading"> 注册 </a-button>
        </form>
        </a-form>
      </div>
    </ThemeProvider>
  </template>
  <script lang="ts" setup>
    import { reactive, ref } from 'vue';
    import { useRouter } from 'vue-router';
    import { useAccountStore } from '@/store/ManagementFeature/account';
    import { ThemeProvider } from 'stepin';
    import { CloseCircleOutlined } from '@ant-design/icons-vue'; // 导入图标组件
  
    export interface SignUpFormProps {
      username: string;
      password: string;
      contact: string;
    }
    const router = useRouter();
    const loading = ref(false);
  
    const form = reactive({
      username: undefined,
      password: undefined,
      contact: undefined,
    });
  
    const emit = defineEmits<{
      (e: 'success', fields: SignUpFormProps): void;
      (e: 'failure', reason: string, fields: SignUpFormProps): void;
    }>();
  
    const accountStore = useAccountStore();
  
    function signUp(params: SignUpFormProps) {
      loading.value = true;
      accountStore
        .signUp(params.username, params.password, params.contact)
        .then((res) => {
          emit('success', params);
        })
        .catch((e) => {
          emit('failure', e.message, params);
        })
        .finally(() => (loading.value = false));
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
  
  