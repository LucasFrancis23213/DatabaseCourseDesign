<template>
    <div>
        <activity-point :current_user="currentUser"  />
    </div>
</template>
<script lang="ts" setup>
import ActivityPoint from '../../components/CommunityFeature/ActivityPoint.vue';
import { useAccountStore } from '@/store/account';
const {account,permissions} = useAccountStore();
let currentUser = {
    id: account.userId,
    name: account.userName,
    avatar: 'src/assets/avatar/face-2.jpg'
};
</script>