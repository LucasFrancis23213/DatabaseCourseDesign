<template>
    <div>
        <activity-point :current_user="currentUser"  />
    </div>
</template>
<script lang="ts" setup>
import ActivityPoint from '../../components/CommunityFeature/ActivityPoint.vue';
let currentUser = {
    id: 1,
    name: 'lzh',
    avatar: 'src/assets/avatar/face-2.jpg'
};
</script>