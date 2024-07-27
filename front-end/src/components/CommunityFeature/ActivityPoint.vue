<template>
    <a-card class="conversations rounded-xl shadow-lg" :bordered="false">
        <div class="chat flex items-center">
            <div class="content ml-3 flex-1">
                <div class="message text-base text-subtext">
                    近期活跃度: {{overallScore}}
                </div>
            </div>
        </div>
      <div class="action">
        <a-button class="text-sm font-semibold" type="link" @click="toggleActicities()">
          {{ activitiesShow ? '隐藏详细' : '查看详细' }}
        </a-button>
      </div>
        <div v-if="activitiesShow" class="activity mt-2 ml-8 p-2 bg-gray-100 rounded-md">
            <div v-for="(activity,aindex) in activitiesList" :key="activity.id" class="chat flex items-center mb-2">
                <div class="ml-3">
                    <div class="type text-sm font-semibold">
                        活跃行为类型: {{ activity.type }}
                    </div>
                    <div class="score text-xs text-subtext">
                        增加活跃度: {{ activity.score }}
                    </div>
                    <div class="time text-xs text-gray-500">
                        {{ activity.time }}
                    </div>
                </div>
            </div>
        </div>
    </a-card>
</template>

<script lang="ts" setup>
import { getBeijingTime,formatTime } from './mytime';
import { ref,reactive,onMounted } from 'vue';
import { Activity } from './type';
import axios from 'axios';
let activitiesShow = ref(false);
let activitiesList = ref<Activity[]>([]);
let overallScore = ref<number>();

// 定义基本访问 URL
//axios.defaults.baseURL = 'https://aa062a3f-a7ef-4a07-be8b-011e24c08aa7.mock.pstmn.io'; // 替换为你的后端 API 地址


const props = defineProps({
  current_user: Object      //需要id，name，avator
});
let current_user_id = props.current_user.id;
const toggleActicities = () => {
  activitiesShow.value = !activitiesShow.value;
};

const fetchOverallScore = async() => {
    try{
        const response = await axios.post('https://7c2b6100-b376-4f68-81d3-7518ce517c93.mock.pstmn.io/api/overallactivity/overall', {current_user_id});
        if(response.data.status === 'success') {
            overallScore.value = response.data.overall_score;
        }
    }catch (error) {
        console.error('Error fetching overall score:', error);
    }
}

const fetchActivities = async() => {
    try {
        const response = await axios.post('https://aa062a3f-a7ef-4a07-be8b-011e24c08aa7.mock.pstmn.io/api/activity/recent', {current_user_id});
        if (response.data.status === 'success') {
            activitiesList.value = response.data.activities;
        }
  } catch (error) {
    console.error('Error fetching activities:', error);
  }
}

onMounted(() => {
  fetchOverallScore();
  fetchActivities();
});

</script>