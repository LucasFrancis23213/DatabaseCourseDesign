<template>
    <div>
    <a-comment
      v-for="(comment, index) in commentsList"
      :key="comment.id"
      :author="comment.user.name"
      :avatar="comment.user.avatar"
      :content="comment.content"
      :datetime="formatTime(comment.time)"
    >
    <template #actions>
        <a v-if="current_user.id === comment.user.id" @click="deleteComment(comment.id)">删除</a>
    </template>
    </a-comment>
  </div>
  <a-form @submit.prevent="addComment" class="mt-4">
    <a-form-item>
      <a-textarea v-model:value="newCommentContent" placeholder="输入您的评论" />
    </a-form-item>
    <a-form-item>
      <a-button type="primary" html-type="submit">提交评论</a-button>
    </a-form-item>
  </a-form>
</template>
<script lang="ts" setup>
import { reactive,ref, onMounted } from 'vue';
import axios from 'axios';
import { Comment } from './type';
//import { testcommentList,current_user } from './type';
import { getBeijingTime,formatTime } from './mytime';

// 定义基本访问 URL
//axios.defaults.baseURL = 'https://localhost:44343'; // 替换为你的后端 API 地址
axios.defaults.baseURL = 'https://acb4d0ca-93ca-4857-bbf7-c389031ad298.mock.pstmn.io';
// 定义从父组件传递的属性
const props = defineProps({
  item_id: String,
  current_user: Object      //需要id，name，avator
});
//需要的数据类型

let item_id = props.item_id;
let current_user = props.current_user;
//let item_id = 123;

let commentsList = ref<Comment[]>([]);    //获取某一item的问题列表
let newCommentContent = ref('');

//调用接口
const fetchComments = async () => {
  try {
    const response = await axios.post('/api/comments/item', { item_id });
    if (response.data.status === 'success') {
      commentsList.value = response.data.comments;
    }
  } catch (error) {
    console.error('Error fetching comments:', error);
  }
};

const addComment = async () => {
  try {
    // 构建请求体
    const newComment = {
      item_id: item_id,
      user_id: current_user.id,
      content: newCommentContent.value,
      time: getBeijingTime()
    };

    // 发送 POST 请求
    const response = await axios.post('/api/comments/add', newComment);

    // 处理响应
    if (response.data.status === 'success') {
      fetchComments();
      newCommentContent.value = '';
    } else {
      console.error('Failed to post comment:', response.data);
    }
  } catch (error) {
    console.error('Error posting comment:', error);
  }
};

const deleteComment = async (comment_id: number) => {
  try {
    const response = await axios.delete(`/api/comments/delete`,{
      params: {
        comment_id: comment_id,
        user_id: current_user.id
      }   
  });
    if (response.data.status === 'success') {
      fetchComments(); // 重新获取问题列表
    }
  } catch (error) {
    console.error('Error deleting comment:', error);
  }
};

onMounted(() => {
  fetchComments();
});
</script>