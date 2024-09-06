export interface Answer {
    id: number;
    user: User;
    content: string;
    time: Date;
    showButton: boolean;

}
  
export interface Question {
    id: number;
    user: User;
    content: string;
    time: Date;
    answers: Answer[];
    showAnswers: boolean;
    showAnswerInput: boolean;
    showButton: boolean;
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


