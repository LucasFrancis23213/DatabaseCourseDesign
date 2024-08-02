import { RouteRecordRaw } from 'vue-router';

const messageRoute:RouteRecordRaw[] = [
  {
    path:'/chat/:conversation_id',
    name:'聊天',
    props:true,
    meta: {
      renderMenu: false,
      cacheable: true,
    },
    component:() =>import('@/pages/CommunityFeature/chat')
  },
  {
    path:'/ConversationList',
    name:'消息',
    props:true,
    meta: {
      renderMenu: true,
      cacheable: true,
    },
    component:() =>import('@/pages/CommunityFeature/ConversationList')
  },
  {
    path:'/ad',
    name:'广告',
    props:true,
    meta: {
      renderMenu: true,
      cacheable: true,
    },
    component:() =>import('@/pages/CommunityFeature/advertisement')
  },
    {
    path:'/vip',
    name:'会员充值',
    props:true,
    meta: {
      renderMenu: true,
      cacheable: true,
    },
    component:() =>import('@/pages/CommunityFeature/vip')
  },
    {
    path:'/adminAd',
    name:'管理广告',
    props:true,
    meta: {
      renderMenu: true,
      cacheable: true,
    },
    component:() =>import('@/pages/admin_CommunityFeature/ad/adminAd.vue')
  },
]

export default messageRoute;