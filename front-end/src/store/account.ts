import { defineStore } from 'pinia';
import http from './http';
import { useLoadingStore } from './loading';
import { useMenuStore } from './menu';
import { useAuthStore } from '@/plugins';
import axios from 'axios';
import { message } from 'ant-design-vue';
axios.defaults.baseURL = import.meta.env.VITE_API_URL;

function getLocalISOTime() {
  const now = new Date();
  const year = now.getFullYear();
  const month = String(now.getMonth() + 1).padStart(2, '0');
  const day = String(now.getDate()).padStart(2, '0');
  const hours = String(now.getHours()).padStart(2, '0');
  const minutes = String(now.getMinutes()).padStart(2, '0');
  const seconds = String(now.getSeconds()).padStart(2, '0');

  return `${year}-${month}-${day}T${hours}:${minutes}:${seconds}`;
}

export interface Profile {
  account: Account;
  permissions: string[];
  role: string;
}
export interface Account {
  userName: string;
  userId:string;
  contact:string;
  avatar:string;
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
      avatar: '',
    } as Account,
    permissions: [],
    role: '',
    logged: true,
  }),
  actions: {
    async login(username, password) {
      this.username = username;
      //useMenuStore().clearMenu();
      const queryParams = new URLSearchParams({ UserName: username, Password: password }).toString();
      try {
        console.log("ready to post request to server");
        const response = await axios.get(`/api/UserManagement/CheckPassword?${queryParams}`);
        console.log("get response! and response status is ",response.status);
        if (response.status === 200) {
          this.logged = true;
          this.account.userName = username;
          if(username ==='admin'){
            this.permissions.push('admin');
          }
          else{
            this.permissions.push('user');
          }
          useAuthStore().setAuthorities(this.permissions);
          http.setAuthorization(`Bearer ${response.data.token}`, new Date(response.data.expires));
          this.profile();
          const logData = {
            user_ID: this.account.userId,
            action_Type: "Login",
            occurrence_Time: getLocalISOTime()
          };
          await axios.post(`/api/LogsInsert/UserOpsLogs`, logData);
          const APIData = {
            apI_Name: "CheckPassword",
            accessor_ID: this.account.userId,
            access_Time: getLocalISOTime(),
            access_Result: "success"
          };
          await axios.post(`/api/LogsInsert/APILogs`, APIData);
          await useMenuStore().getMenuList();
          return { success: true, message: "登录成功！"};
        } 
      } catch (error) {
        const APIData = {
          apI_Name: "CheckPassword",
          accessor_ID: this.account.userId,
          access_Time: getLocalISOTime(),
          access_Result: "failure"
        };
        await axios.post(`/api/LogsInsert/APILogs`, APIData);
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
    async closeApp() {
      return new Promise<boolean>((resolve) => {
        localStorage.removeItem('stepin-menu');
        http.removeAuthorization();
        this.logged = false;
        this.account.userName='';
        this.permissions=[];
        useMenuStore().clearMenu();
        resolve(true);
      });
    },
    async logout() {
      const logData = {
        user_ID: this.account.userId,
        action_Type: "Logout",
        occurrence_Time: getLocalISOTime()
      };
      await axios.post(`/api/LogsInsert/UserOpsLogs`, logData);
      await this.closeApp();
    },
    async profile() {
      const { setAuthLoading } = useLoadingStore();
      setAuthLoading(true);
      if(!this.account.userName){
        return { account: this};
      }
      else{
        try {
          const response = await axios.get(`/api/UserManagement/UserGetUserInfo?UserName=${this.account.userName}`);
          if (response.data && response.data.length > 0) {
            this.account.userName = response.data[0].UserName;
            this.account.userId = response.data[0].UserID;
            this.account.contact = response.data[0].Contact;
            this.account.avatar = response.data[0].Avatar;
            return { success: true, message: "用户信息加载成功", account: this};
          } 
        } catch (error) {
          console.error('Failed to fetch user info:', error);
          return { success: false, message: "用户信息错误", account: this};
        } finally {
          setAuthLoading(false); 
        }
      }
    },
    setLogged(logged: boolean) {
      this.logged = logged;
    },
  async deleteUser() {
    const logData = {
      user_ID: this.account.userId,
      action_Type: "DeleteUser",
      occurrence_Time: getLocalISOTime()
    };
    await axios.post(`/api/LogsInsert/UserOpsLogs`, logData);
    if(!!this.account.userName){
      try {
        await axios.delete(`/api/UserManagement/DeleteUser?UserID=${this.account.userId}`);
        this.account.userName = '';
        this.account.userId = '';
        this.account.contact = '';
        await this.closeApp();
        message.success('已注销');
      } catch (error) {
        message.error('注销失败:',error);
        console.error('Failed to delete user:', error);
      } 
    }
  },
  async signup(username, password, contact) {
    const queryParams = {
      User_Name: username,
      Password: password,
      Contact: contact
    }; 
    try {
      const response = await axios.post(`/api/UserManagement/RegisterUser`, queryParams);
      if (response.status === 200) {
        this.account.userName = username;
        await this.profile();
        const logData = {
          user_ID: this.account.userId, 
          action_Type: "Signup",
          occurrence_Time: getLocalISOTime()
        };
        await axios.post(`/api/LogsInsert/UserOpsLogs`, logData);
        return { success: true, message: "注册成功！即将跳转回登录界面..."};
      }
    } catch (error) {
      if (error.response) {
        if (error.response.status === 500) {
          return { success: false, message: "注册失败，用户名已存在。"};
        } else {
          console.error("Registration error:", error);
          return { success: false, message: `${error.message}`};
        }
      } else {
        console.error("Error:", error);
        return { success: false, message: "网络或服务器错误"};
      }
    }
  },
},
    persist: true,
});