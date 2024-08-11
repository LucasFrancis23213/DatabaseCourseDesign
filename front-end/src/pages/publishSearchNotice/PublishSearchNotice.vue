<script lang="ts" setup>
  import { getBase64 } from '@/utils/file';
  import { ref, onMounted } from 'vue';
  import { FormInstance, message } from 'ant-design-vue';
  import axios from 'axios';
  import dayjs from 'dayjs'

  const baseURL = 'https://localhost:44343/api/';
  
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
  
  // 打乱字符集的函数
  function shuffle(array: string[]) {
    let currentIndex = array.length, randomIndex;

    // While there remain elements to shuffle.
    while (currentIndex !== 0) {
      // Pick a remaining element.
      randomIndex = Math.floor(Math.random() * currentIndex);
      currentIndex--;

      // And swap it with the current element.
      [array[currentIndex], array[randomIndex]] = [
        array[randomIndex], array[currentIndex]];
    }

    return array;
  }

  function generateItemID() {
    const charset = '123456789abcdefghijklmnopqrstuvwxyz'.split('');
    const shuffledCharset = shuffle(charset);
    let uniqueID = '';
    
    // 生成16位ID
    for (let i = 0; i < 16; i++) {
      const randomIndex = Math.floor(Math.random() * shuffledCharset.length);
      uniqueID += shuffledCharset[randomIndex];
    }

    return uniqueID;
  }

  const submit = () => {
    loading.value = true;
    formModel.value?.validateFields()
      .then(async ()=>{
        if (selectedFile.value) {
          const formData = new FormData();
          formData.append('file', selectedFile.value);
          const res = await axios.post(baseURL + 'ItemPicUpload/upload?type=Lost', formData, {
            headers: {
              'Content-Type': 'multipart/form-data',
            },
          });
          form.value.IMAGE_URL = res.data.url;
        }
        
        form.value.ITEM_ID = generateItemID(); // 生成并设置 ITEM_ID
        form.value.CATEGORY_ID = form.value.CATEGORY_ID[0];
        form.value.LOST_DATE = dayjs(form.value.LOST_DATE).format("YYYY-MM-DD HH:mm:ss");
        if (form.value.IS_REWARDED) {
          form.value.DEADLINE = dayjs(form.value.DEADLINE).format("YYYY-MM-DD HH:mm:ss");
        } else {
          form.value.DEADLINE = '无';
        }
        const jsonFormData = JSON.stringify(form.value);
        console.log(jsonFormData);
        await axios.post(baseURL + 'PublishItem/Lost', jsonFormData, {
          headers: {
            'Content-Type': 'application/json',
          },
        });
        getPublishs();
        setTimeout(() => {
          location.reload();
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
    try {
      const res = await axios.get(baseURL + 'QueryItem', {
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
const returnItem = () => {
  returnLoading.value = true
  returnModel.value?.validateFields()
    .then(async () => {
      setTimeout(() => {
        message.success('归还已提交审核！');
        returnLoading.value = false;
        openReturn.value = false
        location.reload()
      }, 2000);
    })
    .catch(() => {
      returnLoading.value = false;
      message.error('提交失败！')
    })
}
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
      <a-form-item label="物品图片" name="IMAGE_URL" has-feedback :rules="[{ required: true, message: '请上传物品图片' }]">
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
 
  <!-- 寻物启事 -->
  <a-table :columns="columns" :dataSource="publishs" >
    <template #title>
      <div class="flex justify-between pr-4">
        <h4>寻物启事</h4>
        <a-button type="primary" @click="addNew" >
          <template #icon>
            <PlusOutlined />
          </template>
          发布
        </a-button>
      </div>
    </template>
    <template #bodyCell="{ column, record, index }">
      <template v-if="column.dataIndex === 'itemNameAndCategory'">
        <div class="text-title font-bold">
          {{ record.ITEM_NAME }}
        </div>
        <div class="text-subtext">
          {{record.CATEGORY_ID}}
        </div>
      </template>
      <template v-else-if="column.dataIndex === 'IMAGE_URL'">
        <img class="w-12 rounded" :src="record.IMAGE_URL" />
      </template>
      <template v-else-if="column.dataIndex === 'IS_REWARDED'">
        <a-badge class="text-subtext" :color="'green'">
          <template #text>
            <span class="text-subtext">{{ IS_REWARDEDDict[Number(record.IS_REWARDED) as IS_REWARDED] }}</span>
          </template>
        </a-badge>
      </template>
      <template v-else-if="column.dataIndex === 'TAG_ID'">
        <span>
          <a-tag
            v-for="tag in record.TAG_ID"
            :key="tag"
            :color="tag === '贵重物品' ? 'volcano' : tag.length > 4 ? 'geekblue' : 'green'"
            >
              {{ tag }}
          </a-tag>
        </span>
      </template>
      <template v-else-if="column.dataIndex === 'RETURN_ITEM'">
        <a-button type="primary" @click="clickReturn(index)">归还</a-button>
      </template>
    </template>
  </a-table>

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
      <a-form-item label="物品图片" name="IMAGE_URL" has-feedback :rules="[{ required: true, message: '请上传图片' }]">
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
        <a-button type="primary" html-type="submit" @click="returnItem" :loading="returnLoading">确认归还</a-button>
      </a-form-item>
    </a-form>
  </a-modal>
</template>
