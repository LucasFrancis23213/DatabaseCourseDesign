<script lang="ts" setup>
  import { getBase64 } from '@/utils/file';
  import { ref, onMounted } from 'vue';
  import { FormInstance, message } from 'ant-design-vue';
  import axios from 'axios';
  import dayjs from 'dayjs'
  import { useAccountStore } from '@/store/account';
  import { generateItemID } from '@/utils/BasicFeature/IDGen';
  import sendSystemMsg from "@/pages/CommunityFeature/chat/systemMsgSend";
  const {account} = useAccountStore();
  axios.defaults.baseURL = import.meta.env.VITE_API_URL;
  
  type onePublish = {
    ITEM_ID?: string;
    ITEM_NAME?: string;
    CATEGORY_ID?: string;
    DESCRIPTION?: string;
    LOST_LOCATION?: string;
    LOST_DATE?: string;
    TAG_ID?:string[];
    IMAGE_URL?: string;
    IS_REWARDED?: boolean;
    REWARD_AMOUNT?: string;
    DEADLINE?: string | '无';
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
      .then(async ()=>{
        if (selectedFile.value) {
          const formData = new FormData();
          formData.append('file', selectedFile.value);
          const res = await axios.post('api/ItemPicUpload/uploadLocal?type=Lost', formData, {
            headers: {
              'Content-Type': 'multipart/form-data',
            },
          });
          form.value.IMAGE_URL = res.data.url;
        }
        
        form.value.ITEM_ID = generateItemID(); // 生成并设置 ITEM_ID
        form.value.USER_ID = account.userId;
        form.value.CATEGORY_ID = form.value.CATEGORY_ID[0];
        form.value.LOST_DATE = dayjs(form.value.LOST_DATE).format("YYYY-MM-DD HH:mm:ss");
        if (form.value.IS_REWARDED) {
          form.value.DEADLINE = dayjs(form.value.DEADLINE).format("YYYY-MM-DD HH:mm:ss");
        } else {
          form.value.DEADLINE = '无';
        }
        const jsonFormData = JSON.stringify(form.value);
        console.log(jsonFormData);
        await axios.post('api/PublishItem/Lost', jsonFormData, {
          headers: {
            'Content-Type': 'application/json',
          },
        });
        getPublishs();
        setTimeout(() => {
          message.success('提交成功！');
          loading.value = false;
          open.value = false;
        }, 2000);
      })
      .catch(()=>{
        loading.value = false;
        message.error('提交失败！');
      })
  };

  const columns = [
    { title: '丢失物品', dataIndex: 'itemNameAndCategory'},
    { title: '物品描述', dataIndex: 'DESCRIPTION', ellipsis: true},
    { title: '遗失地点', dataIndex: 'LOST_LOCATION', ellipsis: true},
    { title: '丢失时间', dataIndex: 'LOST_DATE' },
    { title: '物品标签', dataIndex: 'TAG_ID' },
    { title: '物品图片', dataIndex: 'IMAGE_URL' },
    { title: '是否悬赏', dataIndex: 'IS_REWARDED' },
    { title: '悬赏金额', dataIndex: 'REWARD_AMOUNT' },
    { title: '截止时间', dataIndex: 'DEADLINE' },
    { title: '操作', dataIndex: 'RETURN_ITEM'},
  ];

  const open = ref<boolean>(false);
  const loading = ref<boolean>(false);

  const addNew = () => {
    open.value = true;
  }

  const form = ref<onePublish>({});
  const selectedFile = ref<File | null>(null);

  const handleFileChange = (file : File) => {
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

  type IS_REWARDED = 0 | 1;
  const IS_REWARDEDDict = {
    0: '未悬赏',
    1: '悬赏',
  };

  const publishs = ref([]);

  const getPublishs = async () => {
    console.log(account);
    try {
      const res = await axios.get('api/QueryItem', {
        params: { 
            type: 0,
            review: 1
        }
      });
      
      // 假设返回的数据是一个数组或对象，直接赋值给 publishs
      publishs.value = res.data.map(item => ({
      ...item,
      TAG_ID: [tagMapping[item.TAG_ID as keyof typeof tagMapping] || '未知'],
      CATEGORY_ID: categoryMapping[item.CATEGORY_ID as keyof typeof categoryMapping] || '未知类别'
    }));
      
      console.log('数据获取成功');
      
    } catch (error) {
      console.error('获取数据时出错:', error);
      // 可以在这里添加错误处理逻辑，比如设置一个错误状态
    }
  }
  onMounted(() => getPublishs());

 
const returnModel = ref<FormInstance>();
//点击归还，openClaim变为true显示a-modal
const openReturn = ref<boolean>(false)
//提交归还时的加载
const returnLoading = ref<boolean>(false)
//归还的是finds数组中索引为claimRow的一行
const returnRow = ref()
//归还按钮的点击函数
const clickReturn = (row) => {
  returnRow.value = row
  openReturn.value = true
}
//确认归还的点击函数
const returnItem = async (returnMsg: string, pubUserID: number, claimUserID: number, returnItemID : string) => {
  returnLoading.value = true;
  try {
    await returnModel.value?.validateFields();
    
    // 这里应该添加实际的API调用来处理物品归还
    // 例如:
    // await axios.post(baseURL + 'ReturnItem', {
    //   returnMsg,
    //   pubUserID,
    //   claimUserID,
    //   // 其他必要的数据...
    // });
    sendSystemMsg(pubUserID, returnMsg, claimUserID);
    sendSystemMsg(claimUserID, "点击这条消息进入与失主的聊天", pubUserID);
    var jsonFormData = JSON.stringify({
      "Process_ID" : generateItemID(),
      "ITEM_ID": returnItemID,
      "Publish_User_ID" : pubUserID,
      "Claimant_User_ID" : claimUserID,
    });
    await axios.post('api/claim/ReturnItem', jsonFormData, {
          headers: {
            'Content-Type': 'application/json',
          },
        });
    message.success('归还已提交');
    returnLoading.value = false;
    openReturn.value = false;
    await getPublishs(); // 重新获取最新数据
  } catch (error) {
    console.error('归还提交失败:', error);
    message.error('提交失败！');
    returnLoading.value = false;
  }
};
//填写的留言和上传的图片
const returnForm = ref({
  MESSAGE: '',
  IMAGE_URL: '',
})
</script>

<template>
  <a-modal v-model:visible="open" title="发布寻物启事" >
    <template #footer></template>
    <a-form ref="formModel" :model="form" :labelCol="{ span: 5 }" :wrapperCol="{ span: 16 }" >
      <a-form-item label="物品名称" name="ITEM_NAME" has-feedback :rules="[{ required: true, message: '请输入物品名称' }]">
        <a-input v-model:value="form.ITEM_NAME" :maxlength="20" />
      </a-form-item>
      <a-form-item label="物品类别" name="CATEGORY_ID" has-feedback :rules="[{ required: true, message: '请选择物品类别' }]">
        <a-cascader v-model:value="form.CATEGORY_ID" :options="[{label: '日用品', value: '1',}, {label: '手表', value: '2',},]"/>
      </a-form-item>
      <a-form-item label="物品描述" name="DESCRIPTION" has-feedback :rules="[{ required: true, message: '请输入物品描述' }]">
        <a-textarea :rows="4" v-model:value="form.DESCRIPTION" :maxlength="100" />
      </a-form-item>
      <a-form-item label="丢失地点" name="LOST_LOCATION" has-feedback :rules="[{ required: true, message: '请输入丢失地点' }]">
        <a-textarea :rows="4" v-model:value="form.LOST_LOCATION" :maxlength="100" />
      </a-form-item>
      <a-form-item label="丢失时间" name="LOST_DATE" has-feedback :rules="[{ required: true, message: '请输入丢失时间' }]">
        <a-date-picker v-model:value="form.LOST_DATE" show-time/>
      </a-form-item>
      <a-form-item label="物品标签" name="TAG_ID" has-feedback :rules="[{ required: true, message: '请选择物品标签' }]">
        <a-radio-group v-model:value="form.TAG_ID">
          <a-radio value="1">贵重物品</a-radio>
          <a-radio value="2">私人用品</a-radio>
          <a-radio value="3">医疗用品</a-radio>
        </a-radio-group>
      </a-form-item>
      <a-form-item label="物品图片" name="IMAGE_URL">
        <a-upload :show-upload-list="false" :beforeUpload="(file: File) => extractImg(file)">
          <img class="h-8 p-0.5 rounded border border-dashed border-border" v-if="form.IMAGE_URL" :src="form.IMAGE_URL" />
          <a-button v-else type="dashed">
            <template #icon>
              <UploadOutlined />
            </template>
            上传
          </a-button>
        </a-upload>
      </a-form-item>
      <a-form-item label="是否悬赏" name="IS_REWARDED">
        <a-switch v-model:checked="form.IS_REWARDED" />
      </a-form-item>
      <template v-if="form.IS_REWARDED">
        <a-form-item label="悬赏金额" name="REWARD_AMOUNT" has-feedback>
          <a-input-number v-model:value="form.REWARD_AMOUNT" prefix="￥" :min="1" :max="9999999999" />
        </a-form-item>
        <a-form-item label="悬赏截止时间" name="DEADLINE" has-feedback :rules="[{ required: true, message: '请输入悬赏截止时间' }]">
          <a-date-picker v-model:value="form.DEADLINE" show-time/>
        </a-form-item>
      </template>
      <a-form-item :wrapper-col="{ offset: 10, span: 16 }">
        <a-button type="primary" html-type="submit" @click="submit" :loading="loading">提交</a-button>
      </a-form-item>
    </a-form>
  </a-modal>
 
  <!-- 寻物启事卡片列表 -->
  <div class="lost-items-container">
    <div class="header-actions">
      <h4>寻物启事</h4>
      <a-button type="primary" @click="addNew">
        <template #icon>
          <PlusOutlined />
        </template>
        发布
      </a-button>
    </div>

    <a-row :gutter="[16, 16]">
      <a-col :span="8" v-for="(item, index) in publishs" :key="item.ITEM_ID">
        <a-card hoverable>
          <template #cover>
            <img :alt="item.ITEM_NAME" :src="item.IMAGE_URL" style="height: 200px; object-fit: cover;" />
          </template>
          <a-card-meta :title="item.ITEM_NAME">
            <template #description>
              <p>类别: {{ item.CATEGORY_ID }}</p>
              <p>描述: {{ item.DESCRIPTION }}</p>
              <p>丢失地点: {{ item.LOST_LOCATION }}</p>
              <p>丢失时间: {{ item.LOST_DATE }}</p>
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
              <p>
                悬赏状态: 
                <a-badge :color="'green'">
                  <template #text>
                    <span>{{ IS_REWARDEDDict[Number(item.IS_REWARDED) as IS_REWARDED] }}</span>
                  </template>
                </a-badge>
              </p>
              <p>悬赏金额: ￥{{ item.REWARD_AMOUNT }}</p>
              <p>截止时间: {{ item.DEADLINE }}</p>
            </template>
          </a-card-meta>
          <template #actions>
            <a-button type="primary" @click="clickReturn(index)">归还</a-button>
          </template>
        </a-card>
      </a-col>
    </a-row>
  </div>

  <!-- 归还 -->
  <a-modal v-model:visible="openReturn" title="归还物品">
    <template #footer></template>
    <span style="font-size: medium;">名称：</span><span>{{ publishs[returnRow].ITEM_NAME }}</span><br><br>
    <span style="font-size: medium;">图片：</span><img style="width: 150px;" :src="publishs[returnRow].IMAGE_URL" /><br><br>
    <span style="font-size: medium;">遗失地点：</span><span>{{ publishs[returnRow].LOST_LOCATION }}</span><br><br>
    <span style="font-size: medium;">物品描述：</span><span>{{ publishs[returnRow].DESCRIPTION }}</span><br>
    <hr>
    <a-form ref="claimModel" :model="returnForm" :labelCol="{ span: 5 }" :wrapperCol="{ span: 16 }">
      <a-form-item label="填写留言" name="MESSAGE" has-feedback :rules="[{ required: true, message: '请输入留言' }]">
        <a-textarea :rows="4" v-model:value="returnForm.MESSAGE" :maxlength="150" />
      </a-form-item>
      <a-form-item :wrapper-col="{ offset: 10, span: 16 }">
        <a-button type="primary" html-type="submit" @click="returnItem(returnForm.MESSAGE, +publishs[returnRow].USER_ID, +account.userId, publishs[returnRow].ITEM_ID)" :loading="returnLoading">确认归还</a-button>
      </a-form-item>
    </a-form>
  </a-modal>
</template>
<style scoped>
.lost-items-container {
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