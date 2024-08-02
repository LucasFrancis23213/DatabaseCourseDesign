<script lang="ts" setup>
import reviewSearchNotice from './reviewSearchNotice.vue'
import reviewEnterOldMarket from './reviewEnterOldMarket.vue'
import reviewOldMarketPurchase from './reviewOldMarketPurchase.vue'
import reviewReturnItemMatch from './reviewReturnItemMatch.vue'
import reviewReturnItemProcess from './reviewReturnItemProcess.vue'
import reviewUnclaimedItem from './reviewUnclaimedItem.vue'
import reviewUserPreference from './reviewUserPreference.vue'


</script>
<template>
    <reviewSearchNotice />
    <reviewUnclaimedItem />
    
    <reviewUserPreference />
    
    <reviewReturnItemMatch />
    <reviewReturnItemProcess />

    <reviewEnterOldMarket />
    <reviewOldMarketPurchase />
</template>