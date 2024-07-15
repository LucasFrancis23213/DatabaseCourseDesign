import { RouteRecordRaw } from 'vue-router';

const QandAListroute:RouteRecordRaw = {
    
        path: '/QandAList',
        name: 'QandAList',
        props: true,
        meta: {
          renderMenu: false,
        },
        component: () => import('@/pages/CommunityFeature/QandAList.vue'),
    
}

export default QandAListroute;