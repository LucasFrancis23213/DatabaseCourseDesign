<template>
  <br><br>
  <div class="login flex items-center justify-center">
    <ThemeProvider :color="{ middle: { 'bg-base': '#fff' }, primary: { DEFAULT: '#1896ff' } }">
      <div class="login-box rounded-sm relative">
        <!-- 使用 CloseCircleOutlined 图标作为关闭按钮 -->
        <CloseCircleOutlined class="close-btn absolute top-4 right-4 cursor-pointer text-2xl" @click="closeLogin" />
        <br>
        <a-form
          :model="form"
          :wrapperCol="{ span: 24 }"
          @finish="resetPassword"
          class="login-form w-[400px] p-lg xl:w-[440px] xl:p-xl h-fit text-text"
        >
        <div class="third-platform">
            <div class="third-title mb-md text-lg">重置密码：</div>
          </div>
          <a-form-item :required="true" name="username">
            <a-input
              v-model:value="form.username"
              autocomplete="new-username"
              placeholder="请输入用户名:"
              class="login-input h-[40px]"
            />
            <p style="font-size: 0.8rem; margin-bottom:-20px; color:grey">我们将向该用户名绑定的手机号发送验证码，请注意查收</p>
          </a-form-item>
          <a-form-item :required="true" name="code">
            <a-input
              v-model:value="form.code"
              placeholder="请输入验证码:"
              class="login-input h-[40px]"
            />
            <p v-if="codeError" class="text-red-500" style="font-size: 0.8rem; margin-bottom:-20px">
              验证码错误
            </p>
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
          <a-divider></a-divider>
          <a-button htmlType="submit" class="h-[40px] w-full" type="primary" :loading="loading" :disabled="passwordMismatch"
           :style="passwordMismatch ? { backgroundColor: '#ccc', color: '#666', borderColor: '#aaa' } : {}"> 重置密码 </a-button>
        </a-form>
      </div>
    </ThemeProvider>
  </div>
  </template>
  <script lang="ts" setup>
  import { reactive, ref, computed } from 'vue';
  import axios from 'axios';
  import { useRouter } from 'vue-router';
  import { ThemeProvider } from 'stepin';
  import { CloseCircleOutlined } from '@ant-design/icons-vue';
  import { message } from 'ant-design-vue';
  import { useAccountStore } from '@/store';

  const accountStore = useAccountStore();

  const router = useRouter();
  const loading = ref(false);

  const form = reactive({
    username: '',
    password: '',
    confirmPassword: '',
    code: ''
  });

  const passwordMismatch = computed(() => {
    return form.password !== form.confirmPassword;
  });

  const codeError = computed(() => {
  return form.code && !/^\d{4}$/.test(form.code);
});

  async function resetPassword() {
    if (codeError.value) {
    message.error('验证码格式不正确，请检查后重试');
    return;
  }
    loading.value = true;
    let url = `http://121.36.200.128:5001/api/UserManagement/UpdateUserInfo`;
    accountStore.account.userName=form.username;
    const result = await accountStore.profile();
    if (result.success) {
      message.success(result.message);
    } else {
      message.error(result.message);
      loading.value = false;
      return;
    }
    const resetData = {
      userID: accountStore.account.userId,
      userName: form.username,
      password: form.password,
      "contact": ''
    };
    axios.put(url, resetData)
      .then(() => {
        message.success('重置成功！即将跳转至登录...');
        accountStore.account.userName= '';
        accountStore.account.userId= '';
        accountStore.account.contact= '';
        router.push('/login');
      })
      .catch(error => {
        message.error('重置失败，请重试');
        console.error('Error updating password:', error);
      });
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