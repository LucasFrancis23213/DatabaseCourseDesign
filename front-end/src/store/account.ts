import { defineStore } from 'pinia';
import http from './http';
import { useLoadingStore } from './loading';
import axios from 'axios';
import { Response } from '@/types';
import { useMenuStore } from './menu';
import { useAuthStore } from '@/plugins';

export interface Profile {
  account: Account;
  permissions: string[];
  role: string;
}
export interface Account {
  userName: string;
  userId:string;
  contact:string;
}
export type TokenResult = {
  token: string;
  expires: number;
};

export const useAccountStore = defineStore('account', {
  state: () => ({
    account: {
      userName: '',
      userId: '',
      contact: '',
    } as Account,
    permissions: [],
    role: '',
    logged: true,
  }),
  actions: {
    async login(username, password) {
      this.username = username;
      const queryParams = new URLSearchParams({ UserName: username, Password: password }).toString();
      try {
        const response = await axios.get(`https://localhost:44343/api/CheckPassword?${queryParams}`);
        if (response.status === 200) {
          this.logged = true;
          this.account.userName = username;
          http.setAuthorization(`Bearer ${response.data.token}`, new Date(response.data.expires));
          return { success: true, message: "登录成功！"};
        } 
      } catch (error) {
        if (error.response) {
          if (error.response.status === 401) {
            return { success: false, message: "登录失败：密码错误" };
          } else if (error.response.status === 404) {
            return { success: false, message: "登录失败：用户名不存在，请先注册" };
          } else {
            return { success: false, message: `登录失败：${error.response.statusText}` };
          }
        } else if (error.request) {
          return { success: false, message: "登录失败：服务器无响应" };
        } else {
          return { success: false, message: `登录失败：${error.message}` };
        }
      }
    },
    async logout() {
      return new Promise<boolean>((resolve) => {
        localStorage.removeItem('stepin-menu');
        http.removeAuthorization();
        this.logged = false;
        this.account.userName='';
        resolve(true);
      });
    },
    async profile() {
      const { setAuthLoading } = useLoadingStore();
      setAuthLoading(true);
      if(!this.account.userName){
        return { account: this.account };
      }
      else{
        try {
          const response = await axios.get(`https://localhost:44343/api/GetUserInfo?UserName=${this.account.userName}`);
          if (response.data) {
            this.account.userName = response.data.userName;
            this.account.userId = response.data.userID;
            this.account.contact = response.data.contact;
            return { success: true, message: "用户信息加载成功", account: this.account };
          } 
        } catch (error) {
          console.error('Failed to fetch user info:', error);
          return { account: this.account };
        } finally {
          setAuthLoading(false); // 确保加载状态在操作完成后被重置
        }
      }
    },
    setLogged(logged: boolean) {
      this.logged = logged;
    },
  async deleteUser() {
    if(!!this.account.userName){
      try {
        const response = await axios.get(`https://localhost:44343/api/DeleteUser?UserName=${this.account.userName}`);
        this.account.userName = '';
        this.account.userId = '';
        this.account.contact = '';
        await this.logout();
      } catch (error) {
        console.error('Failed to delete user:', error);
      } 
    }
  },
},
    persist: true,
});
