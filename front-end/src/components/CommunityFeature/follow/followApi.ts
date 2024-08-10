import axios from "axios";
axios.defaults.baseURL = import.meta.env.VITE_API_URL;
import { useAccountStore } from '@/store/account';
const {account, permissions} = useAccountStore();


export const unfollowUser = async (unfollowUserId) => {
  try {
    await axios.post(`/api/user/follow`,{
      user_id:unfollowUserId,
      action:"unfollow",
      current_user_id:account.userId
    })
  } catch (error) {
    console.error('取消关注失败:', error)
    throw error
  }
}