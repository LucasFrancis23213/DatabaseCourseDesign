<template>
    <div class="reset-password-container">
      <a-form
        @finish="handleReset"
        :model="form"
        class="reset-form">
        <a-form-item
          label="手机号码"
          name="phone"
          rules="[...规则]">
          <a-input v-model="form.phone" placeholder="请输入您的手机号码" />
        </a-form-item>
        <a-form-item
          label="验证码"
          name="code"
          rules="[...规则]">
          <a-input v-model="form.code" placeholder="请输入验证码" />
        </a-form-item>
        <a-button type="primary" htmlType="submit">
          重置密码
        </a-button>
      </a-form>
    </div>
  </template>
  
  <script>
  export default {
    data() {
      return {
        form: {
          phone: '',
          code: ''
        }
      };
    },
    methods: {
      handleReset() {
        // 实现重置密码的逻辑
      }
    }
  };
  </script>
  
  <style scoped>
  .reset-password-container {
    max-width: 400px;
    margin: auto;
    padding-top: 50px;
  }
  </style>
  