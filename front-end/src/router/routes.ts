import { RouteRecordRaw } from 'vue-router';

const routes: RouteRecordRaw[] = [
  {
    path: '/',
    name: 'home',
    redirect: '/home',
    meta: {
      title: '首页',
      renderMenu: false,
      icon: 'CreditCardOutlined',
    },
  },
  {
    path: '/front',
    name: '前端',
    meta: {
      renderMenu: false,
    },
    component: () => import('@/components/layout/FrontView.vue'),
    children: [
      {
        path: '/login',
        name: '登录',
        meta: {
          icon: 'LoginOutlined',
          view: 'blank',
          target: '_blank',
          cacheable: false,
        },
        component: () => import('@/pages/login'),
      },
      {
        path: '/home',
        name: '首页',
        meta: {
          view: 'blank',
        },
        component: () => import('@/pages/home'),
      }, 
      {
        path: '/signup',
        name: '注册',
        meta: {
          view: 'blank',
        },
        component: () => import('@/pages/signUp'),
      },
      {
        path: '/reset',
        name: '重置密码',
        meta: {
          view: 'blank',
        },
        component: () => import('@/pages/resetPassword.vue'),
      },
    ],
  },
  {
    path: '/403',
    name: '403',
    props: true,
    meta: {
      renderMenu: false,
    },
    component: () => import('@/pages/Exp403.vue'),
  },
  {
    path: '/:pathMatch(.*)*',
    name: '404',
    props: true,
    meta: {
      icon: 'CreditCardOutlined',
      renderMenu: false,
      cacheable: false,
      _is404Page: true,
    },
    component: () => import('@/pages/Exp404.vue'),
  },
  {
    path: '/:pathMatch(.*)*',
    name: '404',
    props: true,
    meta: {
      icon: 'CreditCardOutlined',
      renderMenu: false,
      cacheable: false,
      _is404Page: true,
    },
    component: () => import('@/pages/Exp404.vue'),
  },
  {
    path: '/PublishSearchNotice',
    name: '寻物启事',
    meta: {
      renderMenu: true,
    },
    component: () => import('@/pages/publishSearchNotice')
  },
  {
    path: '/PublishUnclaimedItem',
    name: '无主物品',
    meta: {
      renderMenu: true,
    },
    component: () => import('@/pages/publishUnclaimedItem')
  },
  {
    path: '/ReturnCenter',
    name: '归还&认领中心',
    meta: {
      renderMenu: true,
    },
    component: () => import('@/pages/returnCenter')
  },
  {
    path: '/Personal',
    name: '个人中心',
    meta: {
      renderMenu: false,
      cacheable: false,
    },
    component: () => import('@/pages/personal')
  },
    {
    path:'/recharge/:recharge_id',
    name:'充值确认',
    props:true,
    meta: {
      renderMenu: false,
      cacheable: false,
      requiresAuth: false,
    },
    component:() =>import('@/pages/CommunityFeature/vipRechargeConfirm.vue')
  },
];
//Community Feature -- QandAList -- 路由添加
import testRoute from './CommunityFeature/test';
testRoute.forEach(route => {
  routes.push(route);
});
//CommunityFeature -- Message路由添加
import messageRoute from "@/router/CommunityFeature/Message";
messageRoute.forEach(route =>{
  routes.push(route);
})
export default routes;
