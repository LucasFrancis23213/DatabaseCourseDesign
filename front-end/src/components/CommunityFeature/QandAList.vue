<template>
  <a-card title="Posts List" class="conversations rounded-xl shadow-lg" :bordered="false">
    
    <!-- 发布帖子输入框和按钮 -->
    <a-form @submit.prevent="addQuestion" class="mb-4">
      <a-form-item>
        <a-textarea v-model:value="newQuestionContent" placeholder="输入您的帖子内容" />
      </a-form-item>
      <a-form-item>
        <a-button type="primary" html-type="submit">发布帖子</a-button>
      </a-form-item>
    </a-form>
    
    <!-- 问题列表 -->
    <div v-for="(question,qindex) in questionsList" :key="question.id" >
      <div class="posts-List">
      <div class="chat flex items-center cursor-pointer" @click="toggleAnswers(question)">
        <img class="w-16 h-16 rounded-xl" :src="question.user.avatar" />
        <div class="content ml-4 flex-1">
          <div class="name text-lg font-semibold">{{ question.user.name }}
            <followButton v-if="question.user.id !== current_user.id" :user_id="question.user.id" :initial-follow-state="false"
                          ></followButton>
          </div>

          <div class="message text-base">{{ question.content }}</div>
          <div class="time text-sm text-gray-500">{{ formatTime(question.time) }}</div>
          <a-button v-if="question.user.id === current_user.id" class="text-sm font-semibold" type="link" @click="deleteQuestion(question.id)">
            撤回
          </a-button>
        </div>
      </div>
      <div v-if="question.showAnswers" class="answers mt-4 ml-16 p-4 bg-gray-100 rounded-md">
        <a-form @submit.prevent="submitAnswer(question)" class="mb-4">
          <a-form-item>
            <a-textarea v-model:value="newAnswerContent" placeholder="输入您的帖子内容" />
          </a-form-item>
          <a-form-item>
            <a-button type="primary" html-type="submit">发布评论</a-button>
          </a-form-item>
        </a-form>
        <div v-for="(answer,aindex) in question.answers" :key="answer.id" class="chat flex items-center mb-4">
          <img class="w-12 h-12 rounded-full" :src="answer.user.avatar" />
          <div class="ml-4">
            <div class="name text-sm font-semibold">{{ answer.user.name }}
            <followButton v-if="answer.user.id !== current_user.id"
                          :user_id="answer.user.id"
                          :initial-follow-state="false"
                          ></followButton></div>
            <div class="message text-base">{{ answer.content }}</div>
            <div class="time text-xs text-gray-500">{{ formatTime(answer.time) }}</div>
          </div>
          <a-button v-if="answer.user.id === current_user.id" class="text-sm font-semibold " type="link" @click="deleteAnswer(question,answer.id,aindex)">
            撤回
          </a-button>
        </div>
      </div>
        </div>
      <advertisement v-if="Math.random() < 0.5"></advertisement>
    </div>
  </a-card>
</template>

<script lang="ts" setup>
import { reactive,ref, onMounted } from 'vue';
import axios from 'axios';

import { Question,Answer,} from './type';
import { getBeijingTime,formatTime } from './mytime';
import followButton from '@/components/CommunityFeature/follow/followButton.vue'
import advertisement from "@/pages/CommunityFeature/advertisement/advertisement.vue";


// 定义从父组件传递的属性
const props = defineProps({
  item_id: String,
  current_user: Object      //需要id，name，avator
});
//需要的数据类型
let item_id = props.item_id;
let current_user = props.current_user;
let questionsList = ref<Question[]>([]);    //获取某一item的问题列表
let newQuestionContent = ref('');           //文本框中输入的问题内容
let newAnswerContent = ref('');             //文本框中输入的回答内容


// 定义基本访问 URL
axios.defaults.baseURL = import.meta.env.VITE_API_URL; // 替换为你的后端 API 地址

//控制 查看回答 按钮的函数
const toggleAnswers = (question: Question) => {
  question.showAnswers = !question.showAnswers;
};

//控制显示 输入答案的对话框 按钮的函数
const toggleAnswerInput = (question: Question) => {
  question.showAnswerInput = !question.showAnswerInput;
};
//
const fetchQuestions = async () => {
  try {
    const response = await axios.post('/api/questions', { item_id });
    if (response.data.status === 'success') {
      questionsList.value = response.data.questions.map((question: Question) => ({
        ...question,
        showAnswers: false, // 设置默认值
        showAnswerInput: false
      }));
    }
  } catch (error) {
    console.error('Error fetching questions:', error);
  }
};



const addQuestion = async () => {
  try {
    const newQuestion = {
      content: newQuestionContent.value,
      item_id: item_id,
      current_user_id: current_user.id, // 替换为实际的用户ID
      time: getBeijingTime()
    };
    const response = await axios.post('/api/post_questions', newQuestion);
    if (response.data.status === 'success') {
      fetchQuestions(); // 重新获取问题列表
      newQuestionContent.value = ''; 
    }
  } catch (error) {
    console.error('Error adding question:', error);
  }
};

const submitAnswer = async (question: Question) => {
  try {
    const newAnswer = {
      question_id: question.id,
      content: newAnswerContent.value,
      current_user_id: current_user.id,
      time: getBeijingTime()
    };
    const response = await axios.post(`/api/questions/${question.id}/post_answers`, newAnswer);
    if (response.data.status === 'success') {
      fetchAnswers(question);
      newAnswerContent.value = ''; // 重置输入框内容
      question.showAnswerInput = false; // 隐藏输入框
      question.showAnswers = true;
    }
    
  } catch (error) {
    console.error('Error submitting answer:', error);
  }
};

const fetchAnswers = async (question:Question) => {
  try {
    const response = await axios.post(`/api/questions/${question.id}/answers`, { question_id:question.id });
    if (response.data.status === 'success') {
      question.answers = response.data.answers;
    }
  } catch (error) {
    console.error('Error fetching answers:', error);
  }
};

const deleteAnswer = async (question: Question,answer_id: number,aindex: number) => {
  try {
    const response = await axios.post(`/api/answers_retract/${answer_id}`, { answer_id:answer_id, current_user_id:current_user.id, time: getBeijingTime() });
    if (response.data.status === 'success') {
      fetchAnswers(question); // 从回答列表中删去该项
    }
  } catch (error) {
    console.error('Error deleting answer:', error);
  }
};

const deleteQuestion = async (question_id: number) => {
  try {
    const response = await axios.post(`/api/questions_retract/${question_id}`, { question_id:question_id, current_user_id:current_user.id, time: getBeijingTime() });
    if (response.data.status === 'success') {
      fetchQuestions(); // 重新获取问题列表
    }
  } catch (error) {
    console.error('Error deleting question:', error);
  }
};


onMounted(() => {
  fetchQuestions();
});

</script>

<style scoped>
.posts-List{
  cursor: pointer;
  border: 1px solid #e8e8e8;
  border-radius: 8px;
  overflow: hidden;
  background-color: #fff;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
  transition: box-shadow 0.3s ease;
  margin-bottom: 16px;
  padding: 10px;
  &:hover {
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
  }
}
</style>