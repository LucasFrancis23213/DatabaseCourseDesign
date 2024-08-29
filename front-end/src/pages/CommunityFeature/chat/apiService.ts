import axios from 'axios';

// 定义一些类型
type Message = {
  id: number | string;
  conversation_id: number;
  content: string;
  type: string;
  time: string;
  sender: number;
  current_user_id: number;
};

type ConversationResponse = {
  messages: Message[];
  name: string;
  avatar: string;
};

// 创建 axios 实例
const api = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
});

// API 服务
const apiService = {
  // 获取消息列表
  getMessages: async (conversationId: number, currentUserId: number): Promise<ConversationResponse> => {
    try {
      const response = await api.post(`/api/conversations/${conversationId}`, {
        conversation_id: conversationId,
        current_user_id: currentUserId,
      });
      return response.data;
    } catch (error) {
      console.error('Failed to get messages:', error);
      throw error;
    }
  },

  // 发送消息
  sendMessage: async (message: Message): Promise<{ message_id: number }> => {
    try {
      const response = await api.post('/api/conversations/messages', message);
      return response.data;
    } catch (error) {
      console.error('Failed to send message:', error);
      throw error;
    }
  },

  // 更新消息的已读状态
  updateReadStatus: async (conversationId: number, currentUserId: number): Promise<void> => {
    try {
      await api.post(`/api/conversations/${conversationId}/update_read_status`, {
        conversation_id: conversationId,
        current_user_id: currentUserId,
      });
    } catch (error) {
      console.error('Failed to update read status:', error);
      throw error;
    }
  },

  // 撤回消息
  retractMessage: async (messageId: number, currentUserId: number): Promise<void> => {
    try {
      await api.post(`/api/messages/${messageId}/retract`, {
        message_id: messageId,
        current_user_id: currentUserId,
        time: new Date().toISOString(),
      });
    } catch (error) {
      console.error('Failed to retract message:', error);
      throw error;
    }
  },

  // 接收消息（如果需要的话）
  receiveMessage: async (messageId: number, receiverInWindow: boolean, senderUserId: number): Promise<void> => {
    try {
      await api.post('/api/messages/receive', {
        message_id: messageId,
        receiver_in_window: receiverInWindow,
        sender_user_id: senderUserId,
      });
    } catch (error) {
      console.error('Failed to receive message:', error);
      throw error;
    }
  },
};

export default apiService;