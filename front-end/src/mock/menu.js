import Mock from 'mockjs';
import { useAccountStore } from '@/store';

const presetList = [
  {
    id: 1,
    name: 'workplace',
    title: '工作台',
    icon: 'DashboardOutlined',
    badge: 'new',
    target: '_self',
    path: '/workplace',
    component: '@/pages/workplace',
    renderMenu: true,
    parent: null,
    permission: null,
    cacheable: true,
  },
  {
    id: 3,
    name: 'personal',
    title: '个人中心',
    path: '/personal',
    icon: 'ProfileOutlined',
    permission: null,
    component: '@/pages/personal',
    renderMenu: true,
    parent: null,
  },
  {
    id: 11,
    name: 'PublishSearchNotice',
    title: 'PublishSearchNotice',
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
    title: 'PublishUnclaimedItem',
    icon: '',
    badge: '',
    target: '_self',
    path: '/PublishUnclaimedItem',
    component: '@/pages/publishUnclaimedItem',
    renderMenu: true,
    parent: null,
    permission: null,
    cacheable: true,
  },
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
    id: 13,
    name: 'adminCenter',
    title: '管理中心',
    icon: 'DashboardOutlined',
    badge: 'new',
    target: '_self',
    path: '/admin',
    component: '@/pages/admin',
    renderMenu: true,
    parent: null,
    permission: 'admin',
    cacheable: true,
  },
  {
    id: 14,
    name: 'BasicFeature',
    title: 'BasicFeature',
    icon: '',
    badge: null,
    target: '_self',
    path: '/admin_BasicFeature',
    component: '@/pages/admin_BasicFeature',
    renderMenu: true,
    parent: null,
    permission: 'admin',
    cacheable: true,
  },
  {
    id: 15,
    name: 'CommunityFeature',
    title: 'CommunityFeature',
    icon: '',
    badge: 3,
    target: '_self',
    path: '/admin_CommunityFeature',
    component: '@/pages/admin_CommunityFeature',
    renderMenu: true,
    parent: null,
    permission: 'admin',
    cacheable: true,
  },
  {
    id: 16,
    name: 'ManagementFeature',
    title: 'ManagementFeature',
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
        renderMenu: true,
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
        renderMenu: true,
        parent: 'ManagementFeature',
        permission: 'admin',
        cacheable: true,
      },
    ],
    permission: 'admin',
    cacheable: true,
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