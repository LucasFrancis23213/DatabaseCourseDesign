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
            <a-input-password
              v-model:value="form.password"
              autocomplete="new-password"
              placeholder="请输入自定义密码:"
              class="login-input h-[40px]"
              type="password"
            />
          </a-form-item>
          <a-form-item>
            <a-input-password
              v-model:value="form.confirmPassword"
              placeholder="请确认自定义密码:"
              class="login-input h-[40px]"
              type="password"
            />
            <p v-if="passwordMismatch" class="text-red-500" style="font-size: 0.8rem; margin-bottom:-20px">密码不一致</p>
          </a-form-item>
          <a-form-item :required="true" name="contact">
            <a-input
              v-model:value="form.contact"
              autocomplete="new-contact"
              placeholder="请输入联系方式（手机号）:"
              class="login-input h-[40px]"
            />
          </a-form-item>
          <div class="terms">
            <label>
              <input type="checkbox" id="agree" name="agree" required="true">
              同意我们的
              <span class="font-bold">用户条款</span>、
              <span class="font-bold">数据使用协议</span>、以及
              <span class="font-bold">Cookie使用协议</span>。
            </label>
          </div>
          <a-divider></a-divider>
          <a-button htmlType="submit" class="h-[40px] w-full" type="primary" :loading="loading" :disabled="passwordMismatch"
           :style="passwordMismatch ? { backgroundColor: '#ccc', color: '#666', borderColor: '#aaa' } : {}"> 注册 </a-button>
        </a-form>
      </div>
    </ThemeProvider>
  </template>
  <script lang="ts" setup>
  import { reactive, ref, computed } from 'vue';
  import { useRouter } from 'vue-router';
  import axios from 'axios'; 
  import { ThemeProvider } from 'stepin';
  import { CloseCircleOutlined } from '@ant-design/icons-vue';
  import { message } from 'ant-design-vue';
  import { useAccountStore } from '@/store';

  const accountStore = useAccountStore();

  interface SignUpFormProps {
    username: string;
    password: string;
    confirmPassword: string;
    contact: string;
  }

  const router = useRouter();
  const loading = ref(false);

  const form = reactive<SignUpFormProps>({
    username: '',
    password: '',
    confirmPassword: '',
    contact: ''
  });

  const emit = defineEmits<{
    (e: 'success', fields: SignUpFormProps): void;
    (e: 'failure', reason: string, fields: SignUpFormProps): void;
  }>();

  const passwordMismatch = computed(() => {
    return form.password !== form.confirmPassword;
  });

  async function signUp() {
    loading.value = true;
    const result = await accountStore.signup(form.username, form.password, form.contact);
    if (result.success) {
        message.success(result.message);
        emit('success', form);
      } else {
        message.error(result.message);
        emit('failure', result.message, form);
      }
    loading.value = false;
  }

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
  
  