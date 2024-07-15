export interface Answer {
    id: number;
    user: User;
    content: string;
    time: Date;
}
  
export interface Question {
    id: number;
    user: User;
    content: string;
    time: Date;
    answers: Answer[];
    showAnswers: boolean;
    showAnswerInput: boolean;
}
export const formatTime = (time: Date) => {
    const date = new Date(time);
    return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
};


//***************************************待完善修改begin****************************************************************************
//假设的数据的定义
//假设User定义
interface User {
    id: number;
    name: string;
    avatar: string;
}
export const item_id = 123; // 你需要根据实际情况设置物品ID
const current_user_id = 1; // 当前用户的ID
export const current_user : User = {
    id: 1,
    name: 'lzh_current',
    avatar: 'src/assets/avatar/face-2.jpg'
}


export const testdata_User1 : User = {
    id: 1,
    name: 'lzh',
    avatar: 'src/assets/avatar/face-2.jpg',
}
export const testdata_User2 : User = {
    id: 2,
    name: 'lzh-2',
    avatar: 'src/assets/avatar/face-2.jpg',
}
export const testdata_A1:Answer = {
    id: 1,
    user: testdata_User1,
    content: 'This is testdata--A1------------------------------so long-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------',
    time: new Date(),
}
export const testdata_A2:Answer = {
    id: 2,
    user: testdata_User2,
    content: 'This is testdata--A2',
    time: new Date(),
}

export const testdata_Q1: Question = {
    id: 1,
    user: testdata_User1,
    content: 'test--Q1--------------------------------------------------------------------------------------------------------so long--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------',
    time: new Date(),
    answers: [testdata_A1,testdata_A2],
    showAnswers: false,
    showAnswerInput: false
}
export const testdata_Q2: Question = {
    id: 1,
    user: testdata_User2,
    content: 'test--Q2',
    time: new Date(),
    answers: [testdata_A1,testdata_A2],
    showAnswers: false,
    showAnswerInput: false
}
import { ref } from "vue";
export const testdata_QL = ref([testdata_Q1,testdata_Q2]);


//***************************************待完善修改end****************************************************************************


