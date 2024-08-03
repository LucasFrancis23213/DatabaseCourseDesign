export function getBeijingTime() {
    // 获取当前 UTC 时间
    let now = new Date();
    
    // 计算北京时间的偏移量（分钟）
    let beijingOffset = 8 * 60;
    
    // 当前本地时间的 UTC 偏移量（分钟）
    let localOffset = now.getTimezoneOffset();
    
    // 计算北京时间
    let beijingTime = new Date(now.getTime() + (beijingOffset + localOffset) * 60000);
    
    return beijingTime;
  }
export const formatTime = (time: Date) => {
    const date = new Date(time);
    return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
};
