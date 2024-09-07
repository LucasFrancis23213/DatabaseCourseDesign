<script lang="ts" setup>
import { getBase64 } from '@/utils/file';
import { ref } from 'vue';
import { FormInstance } from 'ant-design-vue';
import { message } from 'ant-design-vue';
import axios from 'axios';
import { onMounted, onUnmounted } from 'vue';
import dayjs from 'dayjs';
import { useAccountStore } from '@/store/account';
import { generateItemID } from '@/utils/BasicFeature/IDGen';
import sendSystemMsg from "@/pages/CommunityFeature/chat/systemMsgSend";
const {account, permissions} = useAccountStore();
axios.defaults.baseURL = import.meta.env.VITE_API_URL;


type oneFind = {
  ITEM_ID?: string;
  ITEM_NAME?: string;
  CATEGORY_ID?: string;
  DESCRIPTION?: string;
  FOUND_LOCATION?: string;
  FOUND_DATE?: string;
  TAG_ID?: string[];
  IMAGE_URL?: string;
  USER_ID?: string;
};

const tagMapping = {
  1: '贵重物品',
  2: '私人用品',
  3: '医疗用品'
};
const categoryMapping = {
    '1': '日用品',
    '2': '手表',
  };
const formModel = ref<FormInstance>();
const submit = () => {
  loading.value = true;
  formModel.value?.validateFields()
    .then(async () => {
      if (selectedFile.value) {
        const formData = new FormData();
        formData.append('file', selectedFile.value);
        const res = await axios.post('api/ItemPicUpload/uploadLocal?type=Found', formData, {
          headers: {
            'Content-Type': 'multipart/form-data',
          },
        });
        form.value.IMAGE_URL = res.data.url;
      }
      form.value.ITEM_ID = generateItemID(); // 生成并设置 ITEM_ID
      form.value.USER_ID = account.userId;
      form.value.CATEGORY_ID = form.value.CATEGORY_ID[0]
      form.value.FOUND_DATE = dayjs(form.value.FOUND_DATE).format("YYYY-MM-DD HH:mm:ss")
      const jsonFormData = JSON.stringify(form.value)
      console.log(jsonFormData)
      await axios.post('api/PublishItem/Found', jsonFormData, {
        headers: {
          'Content-Type': 'application/json',
        },
      });
      getFinds()
      setTimeout(() => {
        message.success('提交成功！');
        loading.value = false;
        open.value = false;
      }, 2000);
    })
    .catch(() => {
      loading.value = false;
      message.error('提交失败！')
    })
};
const columns = [
  { title: '丢失物品', dataIndex: 'itemNameAndCategory' },
  { title: '物品描述', dataIndex: 'DESCRIPTION', ellipsis: true },
  { title: '发现地点', dataIndex: 'FOUND_LOCATION', ellipsis: true },
  { title: '发现时间', dataIndex: 'FOUND_DATE' },
  { title: '物品标签', dataIndex: 'TAG_ID' },
  { title: '物品图片', dataIndex: 'IMAGE_URL' },
  { title: '操作', dataIndex: 'CLAIM' },
];
const open = ref<boolean>(false);
const loading = ref<boolean>(false);
const addNew = () => {
  open.value = true;
}
const form = ref<oneFind>({
  ITEM_ID: '',
  ITEM_NAME: '',
  CATEGORY_ID: '',
  DESCRIPTION: '',
  FOUND_LOCATION: '',
  FOUND_DATE: '',
  TAG_ID: [],
  IMAGE_URL: '',
  USER_ID: '',
});
const selectedFile = ref<File | null>(null);
const handleFileChange = (file: File) => {
  selectedFile.value = file;
  const reader = new FileReader();
  reader.onload = (e) => {
    form.value.IMAGE_URL = e.target?.result as string;
  };
  reader.readAsDataURL(file);
};
async function extractImg(file: File) {
  handleFileChange(file);
};

const finds = ref([])
const getFinds = async () => {
  try {
    const res = await axios.get('api/QueryItem', {
      params: { 
            type: 1,
            review: 1
        }
    });

    finds.value = res.data.map(item => ({
      ...item,
      TAG_ID: [tagMapping[item.TAG_ID as keyof typeof tagMapping] || '未知'],
      CATEGORY_ID: categoryMapping[item.CATEGORY_ID as keyof typeof categoryMapping] || '未知类别',
    }));

    console.log('数据获取成功');

  } catch (error) {
    console.error('获取数据时出错:', error);
    // 可以在这里添加错误处理逻辑，比如设置一个错误状态
  }
}
onMounted(() => getFinds())


const claimModel = ref<FormInstance>();
//点击认领，openClaim变为true显示a-modal
const openClaim = ref<boolean>(false)
//提交认领时的加载
const claimLoading = ref<boolean>(false)
//认领的是finds数组中索引为claimRow的一行
const claimRow = ref()
//认领按钮的点击函数
const claim = (row) => {
  claimRow.value = row
  openClaim.value = true
}
//确认认领的点击函数
const claimItem = async (returnMsg : string, pubUserID : number, claimUserID : number, claimItemID : string) => {
  claimLoading.value = true
  try {
    await claimModel.value?.validateFields();
    
    // 这里应该添加实际的API调用来处理物品归还
    // 例如:
    // await axios.post(baseURL + 'ReturnItem', {
    //   returnMsg,
    //   pubUserID,
    //   claimUserID,
    //   // 其他必要的数据...
    // });
    sendSystemMsg(pubUserID, returnMsg, claimUserID);
    sendSystemMsg(claimUserID, "点击这条消息进入与发现者的聊天", pubUserID);
    var jsonFormData = JSON.stringify({
      "Process_ID" : generateItemID(),
      "ITEM_ID": claimItemID,
      "Publish_User_ID" : pubUserID,
      "Claimant_User_ID" : claimUserID,
    });
    await axios.post('api/claim/ClaimItem', jsonFormData, {
          headers: {
            'Content-Type': 'application/json',
          },
        });
    message.success('归还已提交');
    claimLoading.value = false;
    openClaim.value = false;
    await getFinds(); // 重新获取最新数据
  } catch (error) {
    console.error('归还提交失败:', error);
    message.error('提交失败！');
    claimLoading.value = false;
  }

}
//填写的留言和上传的图片
const claimForm = ref({
  MESSAGE: '',
  IMAGE_URL: '',
})

let intervalId: ReturnType<typeof setInterval> | null = null;
  
onMounted(() => {
  getFinds(); // Initial call
  intervalId = setInterval(getFinds, 10000); // Call every 10 seconds
});

onUnmounted(() => {
  if (intervalId !== null) {
    clearInterval(intervalId); // Clear the interval when the component is unmounted
  }
});


</script>

<template>
  <a-modal v-model:visible="open" title="发布无主物品">
    <template #footer></template>
    <a-form ref="formModel" :model="form" :labelCol="{ span: 5 }" :wrapperCol="{ span: 16 }">
      <a-form-item label="物品名称" name="ITEM_NAME" has-feedback :rules="[{ required: true, message: '请输入物品名称' }]">
        <a-input v-model:value="form.ITEM_NAME" :maxlength="20" />
      </a-form-item>
      <a-form-item label="物品类别" name="CATEGORY_ID" has-feedback :rules="[{ required: true, message: '请选择物品类别' }]">
        <a-cascader v-model:value="form.CATEGORY_ID"
          :options="[{ label: '日用品', value: '1', }, { label: '手表', value: '2', },]" />
      </a-form-item>
      <a-form-item label="物品描述" name="DESCRIPTION" has-feedback :rules="[{ required: true, message: '请输入物品描述' }]">
        <a-textarea :rows="4" v-model:value="form.DESCRIPTION" :maxlength="100" />
      </a-form-item>
      <a-form-item label="发现地点" name="FOUND_LOCATION" has-feedback :rules="[{ required: true, message: '请输入发现地点' }]">
        <a-textarea :rows="4" v-model:value="form.FOUND_LOCATION" :maxlength="100" />
      </a-form-item>
      <a-form-item label="发现时间" name="FOUND_DATE" has-feedback :rules="[{ required: true, message: '请输入发现时间' }]">
        <a-date-picker v-model:value="form.FOUND_DATE" show-time />
      </a-form-item>
      <a-form-item label="物品标签" name="TAG_ID" has-feedback :rules="[{ required: true, message: '请选择物品标签' }]">
        <a-radio-group v-model:value="form.TAG_ID">
          <a-radio value="1">贵重物品</a-radio>
          <a-radio value="2">私人用品</a-radio>
          <a-radio value="3">医疗用品</a-radio>
        </a-radio-group>
      </a-form-item>
      <a-form-item label="物品图片" name="IMAGE_URL" has-feedback :rules="[{ required: true, message: '请上传物品图片' }]">
        <a-upload :show-upload-list="false" :beforeUpload="(file: File) => extractImg(file)">
          <img class="h-8 p-0.5 rounded border border-dashed border-border" v-if="form.IMAGE_URL"
            :src="form.IMAGE_URL" />
          <a-button v-else type="dashed">
            <template #icon>
              <UploadOutlined />
            </template>
            上传
          </a-button>
        </a-upload>
      </a-form-item>
      <a-form-item :wrapper-col="{ offset: 10, span: 16 }">
        <a-button type="primary" html-type="submit" @click="submit" :loading="loading">提交</a-button>
      </a-form-item>
    </a-form>
  </a-modal>

  <!-- 无主物品卡片列表 -->
  <div class="found-items-container">
    <div class="header-actions">
      <h4>无主物品</h4>
      <a-button type="primary" @click="addNew">
        <template #icon>
          <PlusOutlined />
        </template>
        发布
      </a-button>
    </div>

    <a-row :gutter="[16, 16]">
      <a-col :span="8" v-for="(item, index) in finds" :key="item.ITEM_ID">
        <a-card hoverable>
          <template #cover>
            <img :alt="item.ITEM_NAME" :src="item.IMAGE_URL" style="height: 200px; object-fit: cover;" />
          </template>
          <a-card-meta :title="item.ITEM_NAME">
            <template #description>
              <p>类别: {{ item.CATEGORY_ID }}</p>
              <p>描述: {{ item.DESCRIPTION }}</p>
              <p>发现地点: {{ item.FOUND_LOCATION }}</p>
              <p>发现时间: {{ item.FOUND_DATE }}</p>
              <p>
                标签: 
                <a-tag
                  v-for="tag in item.TAG_ID"
                  :key="tag"
                  :color="tag === '贵重物品' ? 'volcano' : tag.length > 4 ? 'geekblue' : 'green'"
                >
                  {{ tag }}
                </a-tag>
              </p>
            </template>
          </a-card-meta>
          <template #actions>
            <a-button type="primary" @click="claim(index)">认领</a-button>
          </template>
        </a-card>
      </a-col>
    </a-row>
  </div>

  <!-- 认领 -->
  <a-modal v-model:visible="openClaim" title="认领物品">
    <template #footer></template>
    <span style="font-size: medium;">名称：</span><span>{{ finds[claimRow].ITEM_NAME }}</span><br><br>
    <span style="font-size: medium;">图片：</span><img style="width: 150px;" :src="finds[claimRow].IMAGE_URL" /><br><br>
    <span style="font-size: medium;">找到地点：</span><span>{{ finds[claimRow].FOUND_LOCATION }}</span><br><br>
    <span style="font-size: medium;">物品描述：</span><span>{{ finds[claimRow].DESCRIPTION }}</span><br>
    <hr>
    <a-form ref="claimModel" :model="claimForm" :labelCol="{ span: 5 }" :wrapperCol="{ span: 16 }">
      <a-form-item label="填写留言" name="MESSAGE" has-feedback :rules="[{ required: true, message: '请输入留言' }]">
        <a-textarea :rows="4" v-model:value="claimForm.MESSAGE" :maxlength="150" />
      </a-form-item>
      <a-form-item :wrapper-col="{ offset: 10, span: 16 }">
        <a-button type="primary" html-type="submit" @click="claimItem(claimForm.MESSAGE, +finds[claimRow].USER_ID, +account.userId, finds[claimRow].ITEM_ID)" :loading="claimLoading">确认认领</a-button>
      </a-form-item>
    </a-form>
  </a-modal>
</template>
<style scoped>
.found-items-container {
  padding: 20px;
}

.header-actions {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
}

.ant-card-cover img {
  border-radius: 8px 8px 0 0;
}
</style>