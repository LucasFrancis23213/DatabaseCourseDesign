import { ref, onMounted, onUnmounted } from 'vue';

export function useTimeFormat(timestamp: string) {
  const formattedTime = ref(formatTime(timestamp));
  let timer: number | null = null;

  function formatTime(timeString: string): string {
    const now = new Date();
    const time = new Date(timeString);
    const diff = now.getTime() - time.getTime();
    const minutes = Math.floor(diff / 60000);
    const hours = Math.floor(diff / 3600000);
    const days = Math.floor(diff / 86400000);

    if (minutes < 60) {
      return `${minutes}分钟前`;
    } else if (hours < 24) {
      return time.toLocaleTimeString('zh-CN', { hour: '2-digit', minute: '2-digit', second: '2-digit' });
    } else if (days === 1) {
      return '昨天';
    } else if (days < 7) {
      return `${days}天前`;
    } else {
      return '7天前';
    }
  }

  function updateTime() {
    formattedTime.value = formatTime(timestamp);
  }

  onMounted(() => {
    updateTime();
    timer = window.setInterval(updateTime, 60000); // 每分钟更新一次
  });

  onUnmounted(() => {
    if (timer !== null) {
      clearInterval(timer);
    }
  });

  return {
    formattedTime
  };
}