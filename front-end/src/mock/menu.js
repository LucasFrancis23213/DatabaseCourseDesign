import Mock from 'mockjs';
import { useAccountStore } from '@/store';

const presetList = [
  {
    id: 10,
    name: 'github',
    title: '我们的Github仓库',
    icon: 'GithubOutlined',
    badge: 'link',
    target: '_blank',
    path: '/github',
    component: 'link',
    renderMenu: true,
    parent: null,
    cacheable: true,
    link: 'https://github.com/LucasFrancis23213/DatabaseCourseDesign',
  },
  {
    id: 9,
    name: 'personal',
    title: '个人中心',
    icon: '',
    badge: '',
    target: '_self',
    path: '/Personal',
    component: '@/pages/personal',
    renderMenu: true,
    parent: null,
    permission: null,
    cacheable: true,
  },
  {
    id: 11,
    name: 'PublishSearchNotice',
    title: '寻物启事',
    icon: '',
    badge: '',
    target: '_self',
    path: '/PublishSearchNotice',
    component: '@/pages/publishSearchNotice',
    renderMenu: true,
    parent: null,
    permission: null,
    cacheable: true,
  },
  {
    id: 12,
    name: 'PublishUnclaimedItem',
    title: '无主物品',
    icon: '',
    badge: '',
    target: '_self',
    path: '/PublishUnclaimedItem',
    component: '@/pages/publishUnclaimedItem',
    renderMenu: true,
    parent: null,
    permission: null,
    cacheable: false,
  },
    {
    id: 13,
    name: '论坛',
    title: '论坛',
    icon: '',
    badge: '',
    target: '_self',
    path: '/QandAList',
    component: '@/pages/CommunityFeature/QandAList.vue',
    renderMenu: true,
    parent: null,
    permission: null,
    cacheable: false,
  },
    {
    id: 14,
    name: '聊天',
    title: '聊天',
    icon: '',
    badge: '',
    target: '_self',
    path: '/chat/:conversation_id',
    component: '@/pages/CommunityFeature/chat',
    renderMenu: false,
    parent: null,
    permission: null,
    cacheable: false,
  },
    {
    id: 15,
    name: '消息列表',
    title: '消息列表',
    icon: '',
    badge: '',
    target: '_self',
    path: '/ConversationList',
    component: '@/pages/CommunityFeature/ConversationList',
    renderMenu: true,
    parent: null,
    permission: null,
    cacheable: false,
  },
  {
    id: 16,
    name: '寻物启事',
    title: '寻物启事',
    icon: '',
    badge: '',
    target: '_self',
    path: '/PublishSearchNotice',
    component: '@/pages/publishSearchNotice',
    renderMenu: true,
    parent: null,
    permission: null,
    cacheable: false,
  },
  {
    id: 17,
    name: '无主物品',
    title: '无主物品',
    icon: '',
    badge: '',
    target: '_self',
    path: '/PublishUnclaimedItem',
    component: '@/pages/publishUnclaimedItem',
    renderMenu: true,
    parent: null,
    permission: null,
    cacheable: false,
  },
  {
    id: 18,
    name: '归还&认领中心',
    title: '归还&认领中心',
    icon: '',
    badge: '',
    target: '_self',
    path: '/ReturnCenter',
    component: '@/pages/returnCenter',
    renderMenu: true,
    parent: null,
    permission: null,
    cacheable: false,
  },


  // props: true,

];

const adminList = [
  {
    id: 3,
    name: 'personal',
    title: '个人中心',
    path: '/personal',
    icon: 'ProfileOutlined',
    permission: null,
    component: '@/pages/personal',
    renderMenu: false,
    parent: null,
  },
  {
    id: 10,
    name: 'github',
    title: '我们的Github仓库',
    icon: 'GithubOutlined',
    badge: 'link',
    target: '_blank',
    path: '/github',
    component: 'link',
    renderMenu: true,
    parent: null,
    cacheable: true,
    link: 'https://github.com/LucasFrancis23213/DatabaseCourseDesign',
  },
  {
    id: 13,
    name: 'adminCenter',
    title: '管理中心',
    icon: 'DashboardOutlined',
    badge: 'new',
    target: '_self',
    path: '/admin',
    component: '@/pages/admin',
    renderMenu: false,
    parent: null,
    permission: 'admin',
    cacheable: true,
  },
  {
    id: 16,
    name: 'ManagementFeature',
    title: '管理功能',
    icon: '',
    badge: null,
    target: '_self',
    path: '/admin_ManagementFeature',
    component: '@/components/layout/BlankView.vue',
    renderMenu: true,
    parent: null,
    children : [
      {
        id: 17,
        name: 'UserOpsLog',
        title: '用户操作日志',
        icon: '',
        badge: null,
        target: '_self',
        path: '/UserOpsLog',
        component: '@/pages/admin_ManagementFeature/UserOpsLog',
        renderMenu: true,
        parent: 'ManagementFeature',
        permission: 'admin',
        cacheable: true,
      },
      {
        id: 18,
        name: 'SystemLog',
        title: '系统日志',
        icon: '',
        badge: null,
        target: '_self',
        path: '/SystemLog',
        component: '@/pages/admin_ManagementFeature/SystemLog',
        renderMenu: true,
        parent: 'ManagementFeature',
        permission: 'admin',
        cacheable: true,
      },
      {
        id: 19,
        name: 'APILog',
        title: 'API访问日志',
        icon: '',
        badge: null,
        target: '_self',
        path: '/APILog',
        component: '@/pages/admin_ManagementFeature/APILog',
        renderMenu: true,
        parent: 'ManagementFeature',
        permission: 'admin',
        cacheable: true,
      },
      {
        id: 20,
        name: 'SecurityEvent',
        title: '安全事件记录',
        icon: '',
        badge: null,
        target: '_self',
        path: '/SecurityEvent',
        component: '@/pages/admin_ManagementFeature/SecurityEvent',
        renderMenu: true,
        parent: 'ManagementFeature',
        permission: 'admin',
        cacheable: true,
      },
      {
        id: 21,
        name: 'TransactionLog',
        title: '交易记录与管理',
        icon: '',
        badge: null,
        target: '_self',
        path: '/TransLog',
        component: '@/pages/admin_ManagementFeature/TransLog',
        renderMenu: false,
        parent: 'ManagementFeature',
        permission: 'admin',
        cacheable: true,
      },
      {
        id: 22,
        name: 'GetUserInfo',
        title: '用户信息查询',
        icon: '',
        badge: null,
        target: '_self',
        path: '/GetUserInfo',
        component: '@/pages/admin_ManagementFeature/GetUserInfo',
        renderMenu: true,
        parent: 'ManagementFeature',
        permission: 'admin',
        cacheable: true,
      },
      {
        id: 23,
        name: 'NotificationLog',
        title: '通知查询',
        icon: '',
        badge: null,
        target: '_self',
        path: '/NotificationLog',
        component: '@/pages/admin_ManagementFeature/NotificationLog',
        renderMenu: true,
        parent: 'ManagementFeature',
        permission: 'admin',
        cacheable: true,
      },
      {
        id: 24,
        name: 'RecommendationLog',
        title: '推荐查询',
        icon: '',
        badge: null,
        target: '_self',
        path: '/RecommendationLog',
        component: '@/pages/admin_ManagementFeature/RecommendationLog',
        renderMenu: false,
        parent: 'ManagementFeature',
        permission: 'admin',
        cacheable: true,
      },
        {
        id: 25,
        name: 'RecommendationLog',
        title: '管理广告',
        icon: '',
        badge: null,
        target: '_self',
        path: '/adminAd',
        component: '@/pages/admin_CommunityFeature/ad/adminAd.vue',
        renderMenu: true,
        parent: 'ManagementFeature',
        permission: 'admin',
        cacheable: true,
      },

    ],
    permission: 'admin',
    cacheable: true,
  },
    {
    id: 26,
    name: '论坛',
    title: '论坛',
    icon: '',
    badge: '',
    target: '_self',
    path: '/QandAList',
    component: '@/pages/CommunityFeature/QandAList.vue',
    renderMenu: true,
    parent: null,
    permission: 'admin',
    cacheable: false,
  },
    {
    id: 27,
    name: '聊天',
    title: '聊天',
    icon: '',
    badge: '',
    target: '_self',
    path: '/chat/:conversation_id',
    component: '@/pages/CommunityFeature/chat',
    renderMenu: false,
    parent: null,
    permission: 'admin',
    cacheable: false,
  },
    {
    id: 28,
    name: '消息列表',
    title: '消息列表',
    icon: '',
    badge: '',
    target: '_self',
    path: '/ConversationList',
    component: '@/pages/CommunityFeature/ConversationList',
    renderMenu: true,
    parent: null,
    permission: 'admin',
    cacheable: false,
  },
  {
    id: 29,
    name: '寻物启事',
    title: '寻物启事',
    icon: '',
    badge: '',
    target: '_self',
    path: '/PublishSearchNotice',
    component: '@/pages/publishSearchNotice',
    renderMenu: true,
    parent: null,
    permission: null,
    cacheable: false,
  },
  {
    id: 30,
    name: '无主物品',
    title: '无主物品',
    icon: '',
    badge: '',
    target: '_self',
    path: '/PublishUnclaimedItem',
    component: '@/pages/publishUnclaimedItem',
    renderMenu: true,
    parent: null,
    permission: null,
    cacheable: false,
  },
  {
    id: 31,
    name: '归还&认领中心',
    title: '归还&认领中心',
    icon: '',
    badge: '',
    target: '_self',
    path: '/ReturnCenter',
    component: '@/pages/returnCenter',
    renderMenu: true,
    parent: null,
    permission: null,
    cacheable: false,
  },
  {
    id: 32,
    name: '审核中心',
    title: '审核中心',
    icon: '',
    badge: '',
    target: '_self',
    path: '/Review',
    component: '@/pages/review',
    renderMenu: true,
    parent: null,
    permission: null,
    cacheable: false,
  },
];

function getMenuList() {
  const {permissions}=useAccountStore();
  const menuStr = localStorage.getItem('stepin-menu');
  let menuList = [];
  if(permissions.includes('admin')){
    menuList = adminList;
    localStorage.setItem('stepin-menu', JSON.stringify(menuList));
  } else if (!menuStr) {
    menuList = presetList;
    localStorage.setItem('stepin-menu', JSON.stringify(menuList));
  } else {
    menuList = JSON.parse(menuStr);
  }
  return menuList;
}

function saveMenu(menu) {
  const menuList = getMenuList();
  if (!menu.id) {
    menu.id = menuList.map((item) => item.id).reduce((p, c) => Math.max(p, parseInt(c)), 0) + 1;
  }
  const index = menuList.findIndex((item) => item.id === menu.id);
  if (index === -1) {
    menuList.push(menu);
  } else {
    menuList.splice(index, 1, menu);
  }
  localStorage.setItem('stepin-menu', JSON.stringify(menuList));
}

Mock.mock('api/menu', 'get', ({}) => {
  let menuList = getMenuList();
  const menuMap = menuList.reduce((p, c) => {
    p[c.name] = c;
    return p;
  }, {});
  menuList.forEach((menu) => {
    menu.renderMenu = !!menu.renderMenu;
    if (menu.parent) {
      const parent = menuMap[menu.parent];
      parent.children = parent.children ?? [];
      parent.children.push(menu);
    }
  });
  return {
    message: 'success',
    code: 0,
    data: menuList.filter((menu) => !menu.parent),
  };
});

Mock.mock('api/menu', 'put', ({ body }) => {
  const menu = JSON.parse(body);
  saveMenu(menu);
  return {
    code: 0,
    message: 'success',
  };
});

Mock.mock('api/menu', 'post', ({ body }) => {
  const menu = JSON.parse(body);
  saveMenu(menu);
  return {
    code: 0,
    message: 'success',
  };
});

Mock.mock('api/menu', 'delete', ({ body }) => {
  const id = body.get('id')[0];
  let menuList = getMenuList();
  const index = menuList.findIndex((menu) => menu.id === id);
  const [removed] = menuList.splice(index, 1);
  localStorage.setItem('stepin-menu', JSON.stringify(menuList));
  return {
    code: 0,
    message: 'success',
    data: removed,
  };
});
