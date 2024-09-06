// 打乱字符集的函数
function shuffle(array: string[]) {
    let currentIndex = array.length, randomIndex;
  
    // While there remain elements to shuffle.
    while (currentIndex !== 0) {
      // Pick a remaining element.
      randomIndex = Math.floor(Math.random() * currentIndex);
      currentIndex--;
  
      // And swap it with the current element.
      [array[currentIndex], array[randomIndex]] = [
        array[randomIndex], array[currentIndex]];
    }
  
    return array;
  }
  
  export function generateItemID() {
    const charset = '123456789abcdefghijklmnopqrstuvwxyz'.split('');
    const shuffledCharset = shuffle(charset);
    let uniqueID = '';
  
    // 生成16位ID
    for (let i = 0; i < 16; i++) {
      const randomIndex = Math.floor(Math.random() * shuffledCharset.length);
      uniqueID += shuffledCharset[randomIndex];
    }
  
    return uniqueID;
  }
  