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
    avatar: account.avatar,
};
</script>