<script lang="ts" setup>
import reviewSearchNotice from './components/reviewSearchNotice.vue'
import reviewEnterOldMarket from './components/reviewEnterOldMarket.vue'
import reviewOldMarketPurchase from './components/reviewOldMarketPurchase.vue'
import reviewReturnItem from './components/reviewReturnItem.vue'
import reviewUnclaimedItem from './components/reviewUnclaimedItem.vue'
import reviewUserPreference from './components/reviewUserPreference.vue'


</script>
<template>
    <h2 style="font-weight: bold;">管理员审核中心</h2>
    <hr>
    <reviewSearchNotice />
    <hr>
    <reviewUnclaimedItem />
    <hr>
    <reviewUserPreference />
    <hr>
    <reviewReturnItem />
    <hr>
    <reviewEnterOldMarket />
    <hr>
    <reviewOldMarketPurchase />
</template>