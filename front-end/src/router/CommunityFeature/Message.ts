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
    {
    path:'/adminvip',
    name:'管理vip(test)',
    props:true,
    meta: {
      renderMenu: true,
      cacheable: true,
    },
    component:() =>import('@/pages/admin_CommunityFeature/vip/vipTest.vue')
  },
    {
    path:'/follow',
    name:'你的关注',
    props:true,
    meta: {
      renderMenu: true,
      cacheable: true,
    },
    component:() =>import('@/pages/CommunityFeature/follow/follow.vue')
  },
    {
    path:'/fans',
    name:'你的粉丝',
    props:true,
    meta: {
      renderMenu: true,
      cacheable: true,
    },
    component:() =>import('@/components/CommunityFeature/follow/fansList.vue')
  },
]

export default messageRoute;