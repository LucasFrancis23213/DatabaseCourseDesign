import { defineStore } from 'pinia';
import http from '../http';
import { Response } from '@/types';
import { useMenuStore } from '../menu';

export interface Account {
  username: string;
  avatar: string;
  gender: number;
}

export type TokenResult = {
  token: string;
  expires: number;
};

export const useAccountStore = defineStore('account', {
  state() {
    return {
      account: {} as Account,
      permissions: [] as string[],
      role: '',
      logged: true,
    };
  },
  actions: {
    async signUp(username: string, password: string, contact:string) {
      return http
        .request<TokenResult, Response<TokenResult>>('/signUp', 'post_json', { username, password, contact })
        .then(async (response) => {
          if (response.code === 0) {
            this.logged = true;
            http.setAuthorization(`Bearer ${response.data.token}`, new Date(response.data.expires));
            await useMenuStore().getMenuList();
            return response.data;
          } else {
            return Promise.reject(response);
          }
        });
    },
  },
});
