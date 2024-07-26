import { RouteRecordRaw } from 'vue-router';

const testRoute:RouteRecordRaw[] = [
  {
    path: '/QandAList',
    name: 'QandAList',
    props: true,
    meta: {
    renderMenu: false,
    },
    component: () => import('@/pages/CommunityFeature/QandAList.vue'),
  },
  {
    path: '/CommentsList',
    name: 'CommentsList',
    props: true,
    meta: {
    renderMenu: false,
    },
    component: () => import('@/pages/CommunityFeature/CommentsList.vue'),
  },
  {
    path: '/ActivityPoint',
    name: 'ActivityPoint',
    props: true,
    meta: {
      renderMenu: false,
    },
    component: () => import('@/pages/CommunityFeature/ActivityPoint.vue'),
  }
]

export default testRoute;