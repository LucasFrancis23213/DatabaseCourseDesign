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
export interface Comment {
    id: number;
    user: User;
    content: string;
    time: Date;
}
export interface Activity {
    id: number;
    type: string;
    score: number;
    time: Date;
}
interface User {
    id: number;
    name: string;
    avatar: string;
}


//***************************************待完善修改begin****************************************************************************
//假设的数据的定义
//假设User定义


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
import { getBeijingTime } from "./mytime";
export const testdata_QL = ref([testdata_Q1,testdata_Q2]);

const testcomment1 : Comment = {
    id: 1,
    user: testdata_User1,
    content: "this testdata comment1",
    time: getBeijingTime()
}
const testcomment2: Comment = {
    id: 2,
    user: testdata_User2,
    content: "this testdata comment2",
    time: getBeijingTime()
}
export const testcommentList = ref([testcomment1,testcomment2])

//***************************************待完善修改end****************************************************************************


