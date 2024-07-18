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
        </a-form>
      </div>
    </ThemeProvider>
  </template>
  <script lang="ts" setup>
  import { reactive, ref } from 'vue';
  import { useRouter } from 'vue-router';
  import axios from 'axios';  // 引入 axios
  import { ThemeProvider } from 'stepin';
  import { CloseCircleOutlined } from '@ant-design/icons-vue';
  import { message } from 'ant-design-vue';

  interface SignUpFormProps {
    username: string;
    password: string;
    contact: string;
  }

  const router = useRouter();
  const loading = ref(false);

  const form = reactive<SignUpFormProps>({
    username: '',
    password: '',
    contact: ''
  });

  const emit = defineEmits<{
    (e: 'success', fields: SignUpFormProps): void;
    (e: 'failure', reason: string, fields: SignUpFormProps): void;
  }>();

  function signUp() {
    loading.value = true;
    axios.post('https://localhost:7116/api/Register', {
      User_Name: form.username,
      Password: form.password,
      Contact: form.contact
    })
    .then(response => {
      message.success("注册成功！即将跳转回登录界面...");
      emit('success', form);
    })
    .catch(error => {
      if (error.response.status === 500) {
        message.error("注册失败，用户名已存在。");
      }else{
        message.error(error.message);
        console.error("Registration error:", error);
      }
      emit('failure', error.response.data, form);
    })
    .finally(() => {
      loading.value = false;
    });
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
  
  