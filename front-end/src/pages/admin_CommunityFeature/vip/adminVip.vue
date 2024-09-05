<script setup lang="ts">
import vipAddItem from '@/components/CommunityFeature/vip/vipAddItem.vue'
import vipDetails from '@/components/CommunityFeature/vip/vipDetails.vue'
import {onMounted, ref} from "vue";
import axios from "axios";


const props=defineProps({
  user_id: {//聊天内容字符串
    type: Number,
    required: true
  }
});
console.log(props.user_id)
const isModalVisible = ref(false);

const showModal = async () => {
  isModalVisible.value = true;
  memberStatus.value = await isMember(props.user_id);
};

const handleOk = () => {
  isModalVisible.value = false;
};

const handleCancel = () => {
  isModalVisible.value = false;
};

const isMember = async (user_id)=>{
  try{
    const res =await axios.post("/api/vip/isMember",{
      user_id:user_id
    })
    return res.data.is_vip;
  }catch (e){
    console.error(e);
  }
}
const memberStatus=ref(false);
onMounted(async ()=>{
  memberStatus.value = await isMember(props.user_id);

})

</script>

<template>
  <a-button @click="showModal">打开 VIP 详情</a-button>
  <a-modal v-model:visible="isModalVisible" title="VIP 详情" @ok="handleOk" @cancel="handleCancel">
     <vipDetails v-if="memberStatus" :user_id='user_id'></vipDetails>
    <vipAddItem v-else :user_id="user_id"></vipAddItem>

  </a-modal>
</template>
<style scoped lang="less">

</style>