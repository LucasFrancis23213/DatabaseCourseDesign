<script lang ="ts" setup>
/*QuestionsList 为问题列表，其包含属性：
* 帖子ID(item_id)若干问题(question)
* * 每个问题包含属性：
* * 问题ID(id)、问题提出人的name(user_name)、问题提出人的头像(user_avatar)、问题内容(content)、问题的回答列表(AnswersList)
* * *问题的回答列表包含属性：
* * *回答ID(id)、回答提出人的name(user_name)、回答提出人的头像(user_avatar)、回答内容(content)。
*/
import { reactive } from 'vue';

interface Answer {
  id: number;
  user_name: string;
  user_avatar: string;
  content: string;
}

interface Question {
  id: number;
  user_name: string;
  user_avatar: string;
  content: string;
  AnswersList: Answer[];
}

const Answer1: Answer = reactive({
  id: 1,
  user_name: 'lzh',
  user_avatar: 'src/assets/avatar/face-2.jpg',
  content: 'hello, world',
});

const Question1: Question = reactive({
  id: 1,
  user_name: 'lzh',
  user_avatar: 'src/assets/avatar/face-2.jpg',
  content: 'hello, world!',
  AnswersList: [Answer1],
});
const QuestionsList = reactive([Question1]);/*
import { reactive } from 'vue';
const Answer1 = reactive({
  user_name: 'lzh',
  user_avatar: 'src/assets/avatar/face-2.jpg',
  content: 'hello,world',
})
const Question1 = reactive({
  id: 1,
  uesr_name: 'lzh',
  user_avatar: 'src/assets/avatar/face-2.jpg',
  content: 'hello,world!',
  AnswersList: [Answer1]
})

*/
import { useRouter } from 'vue-router';
const router = useRouter();
const Questionclick = () => {
  router.push({path: '/AnsList',query: {AnsList: JSON.stringify(Question1.AnswersList)}}); // '/new-page' 是你想导航到的路由路径
};

</script>
<template>
  <a-card title="Questions List" class="conversations rounded-xl shadow-lg" :bordered="false">
    <div class="chat flex items-center" v-for="(Question,index) in QuestionsList">
      <img class="w-12 rounded-xl" :src="Question.user_avatar" />
      <div class="content ml-3 flex-1">
        <div class="name">{{ Question.user_name }}</div>
        <div class="message text-sm text-subtext">{{ Question.content }}</div>
      </div>
      <div class="action">
        <a-button class="text-sm font-semibold" type="link" @click = 'Questionclick'>回答详细</a-button>
      </div>
    </div>
  </a-card>
</template>
<style>
</style>