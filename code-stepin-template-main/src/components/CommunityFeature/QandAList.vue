<template>
  <a-card title="Questions List" class="conversations rounded-xl shadow-lg" :bordered="false">
    <div v-for="(question,qindex) in questionsList" :key="question.id">
      <div class="chat flex items-center">
      <img class="w-12 rounded-xl" :src="question.user.avatar" />
      <div class="content ml-3 flex-1">
        <div class="name">{{ question.user.name }}</div>
        <div class="message text-sm text-subtext">{{ question.content }}</div>
        <div class="time text-xs text-gray-500">{{ formatTime(question.time) }}</div>
      </div>
      <div class="action">
        <a-button v-if="question.user.id === current_user.id" class="text-sm font-semibold" type="link" @click="deleteQuestion(question.id)">
          撤回
        </a-button>
        <a-button class="text-sm font-semibold" type="link" @click="toggleAnswers(question)">
          {{ question.showAnswers ? '隐藏回答' : '查看回答' }}
        </a-button>
        <a-button class="text-sm font-semibold" type="link" @click="toggleAnswerInput(question)">
          {{ question.showAnswerInput ? '收起解答' : '解答问题' }}
        </a-button>
      </div>
    </div>
    <div v-if="question.showAnswerInput" class="answer-input mt-2 ml-8 p-2 bg-gray-100 rounded-md">
        <a-textarea v-model:value="newAnswerContent" placeholder="输入您的回答" />
        <a-button class="mt-2" type="primary" @click="submitAnswer(question)">提交回答</a-button>
    </div>
      <div v-if="question.showAnswers" class="answers mt-2 ml-8 p-2 bg-gray-100 rounded-md">
        <div v-for="(answer,aindex) in question.answers" :key="answer.id" class="chat flex items-center mb-2">
          <img class="w-8 h-8 rounded-full" :src="answer.user.avatar" />
          <div class="ml-3">
            <div class="name text-sm font-semibold">{{ answer.user.name }}</div>
            <div class="message text-xs text-subtext">{{ answer.content }}</div>
            <div class="time text-xs text-gray-500">{{ formatTime(answer.time) }}</div>
          </div>
          <div class="action">
          <a-button v-if="answer.user.id === current_user.id" class="text-sm font-semibold " type="link" @click="deleteAnswer(question,answer.id,aindex)">
            撤回
          </a-button>    
          </div>
        </div>
      </div>
    </div>   
  </a-card>
  <a-form @submit.prevent="addQuestion" class="mt-4">
    <a-form-item>
      <a-textarea v-model:value="newQuestionContent" placeholder="输入您的问题" />
    </a-form-item>
    <a-form-item>
      <a-button type="primary" html-type="submit">提交问题</a-button>
    </a-form-item>
  </a-form>
</template>

<script lang="ts" setup>
import { reactive,ref, onMounted } from 'vue';
import axios from 'axios';

import { Question,Answer,} from './type';
import { getBeijingTime,formatTime } from './mytime';


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
axios.defaults.baseURL = 'https://localhost:44343'; // 替换为你的后端 API 地址

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

</style>