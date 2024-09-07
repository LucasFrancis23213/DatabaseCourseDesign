import {useRouter} from "vue-router";
const router = useRouter();

import { useAccountStore } from '@/store/account';
const {account, permissions} = useAccountStore();
export const openConversationInput = (target_id) => {
  router.push({
    name: "聊天",
    params: {
      "conversation_id": target_id,
    },
    query: { "current_user_id":account.userId, }
  })
};