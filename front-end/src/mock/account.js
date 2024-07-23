import Mock from 'mockjs';
import CryptoJS from 'crypto-js';
import dayjs from 'dayjs';

function base64UrlEncode(source) {
  return CryptoJS.enc.Base64.stringify(source).replace(/=+$/, '').replace(/\+/g, '-').replace(/\//g, '_');
}

function sign(payload, salt, { expiresIn, encode = 'utf8' }) {
  const exp = new Date().getTime() + expiresIn;
  const header = JSON.stringify({ alg: 'HS256', type: 'JWT', exp });
  const payloadStr = JSON.stringify({ ...payload, exp });

  const base64Str =
    base64UrlEncode(CryptoJS.enc.Utf8.parse(header)) + '.' + base64UrlEncode(CryptoJS.enc.Utf8.parse(payloadStr));
  const signature = base64UrlEncode(CryptoJS.HmacSHA256(base64Str, salt));
  return base64Str + '.' + signature;
}

Mock.mock('api/login', 'post', ({ body }) => {
  const { username, password } = JSON.parse(body ?? '{}');
  if (username === 'admin' && password === '888888') {
    const expiresIn = 24 * 60 * 60 * 1000;
    const token = sign({ username: 'admin', role: 'admin' }, 'secret key', { expiresIn });
    return { code: 0, message: 'success', data: { token, expires: expiresIn + new Date().getTime() } };
  } else {
    return { code: 401, message: '用户名或密码错误' };
  }
});

Mock.mock('api/account', 'get', ({}) => {
  return {
    code: 0,
    message: 'success',
    data: {
      account: {
        username: 'iczer',
        age: 18,
        gender: 0,
        avatar: 'http://portrait.gitee.com/uploads/avatars/user/691/2073535_iczer_1578965604.png!avatar30',
      },
      role: 'admin',
      permissions: ['edit', 'delete', 'add'],
    },
  };
});
const arr = [
  {
    "Category_ID": '手表',
    "Description": '劳力士',
    "Lost_Location": '公园',
    "Lost_Date": '2024-07-01 15:30:48',
    "Tag_ID":['贵重物品', '私人用品', '医疗用品', '66666666'],
    "Image_URL": '/src/assets/avatar/face-2.jpg',
    "isRewarded": true,
    "Reward_Amount": '80000',
    "Deadline": '2024-07-01 15:30:48',
    "Item_Name": 'Li Zhi',
  },
  {
    "Item_Name": 'Li Zhi',
    "Category_ID": '手表',
    "Description": '劳力士',
    "Lost_Location": '公园',
    "Lost_Date": '2024-07-01 15:30:48',
    "Tag_ID":['贵重物品'],
    "Image_URL": '/src/assets/avatar/face-1.jpg',
    "isRewarded": false,
    "Reward_Amount": '0',
    "Deadline": '无',
  },
]
Mock.mock('api/publishs', 'get', () => arr)
Mock.mock('api/addNewPublish', 'post', function(options){
  const requestData = JSON.parse(options.body)
  arr.push(requestData)
})
const arr2 = [
  {
    "Category_ID": '手表',
    "Description": '劳力士',
    "findPosition": '公园',
    "findTime": '2024-07-01 15:30:48',
    "Tag_ID":['贵重物品', '私人用品', '医疗用品', '66666666'],
    "Image_URL": '/src/assets/avatar/face-2.jpg',
    "Item_Name": 'Li Zhi',
  },
  {
    "Item_Name": 'Li Zhi',
    "Category_ID": '手表',
    "Description": '劳力士',
    "findPosition": '公园',
    "findTime": '2024-07-01 15:30:48',
    "Tag_ID":['贵重物品'],
    "Image_URL": '/src/assets/avatar/face-1.jpg',
  },
]
Mock.mock('api/finds', 'get', () => arr2)
Mock.mock('api/addNewFind', 'post', function(options){
  const requestData = JSON.parse(options.body)
  arr2.push(requestData)
})
