<script lang="ts" setup>
  import { ref, onMounted } from 'vue';
  import { FormInstance, message } from 'ant-design-vue';
  import axios from 'axios';
  import dayjs from 'dayjs'

  const baseURL = 'http://121.36.200.128:5000/api/';

  const categoryMap = {
    '1': '类别一',
    '2': '手表',
  }
  const tagMap = {
    '1': '贵重物品',
    '2': '私人物品',
    '3': '医疗用品',
  }

  type onePublish = {
    ITEM_ID?: string;
    ITEM_Name?: string;
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
        
        form.value.ITEM_ID = generateItemID(); // 生成并设置 Item_ID
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
    { title: '丢失物品', dataIndex: 'ITEM_NAME_CATEGORY'},
    { title: '物品描述', dataIndex: 'DESCRIPTION', ellipsis: true},
    { title: '遗失地点', dataIndex: 'LOST_LOCATION', ellipsis: true},
    { title: '丢失时间', dataIndex: 'LOST_DATE' },
    { title: '物品标签', dataIndex: 'TAG_ID' },
    { title: '物品图片', dataIndex: 'IMAGE_URL' },
    { title: '是否悬赏', dataIndex: 'IS_REWARDED' },
    { title: '悬赏金额', dataIndex: 'REWARD_AMOUNT' },
    { title: '截止时间', dataIndex: 'DEADLINE' },
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

  type IsRewarded = 0 | 1;
  const IsRewardedDict = {
    0: '未悬赏',
    1: '悬赏',
  };

  const publishs = ref([]);

  const getPublishs = async () => {
    try {
      const res = await axios.get(baseURL + 'QueryItem', {
        params: { type: 0 }
      });
      
      // 假设返回的数据是一个数组或对象，直接赋值给 publishs
      publishs.value = res.data;
      
      console.log('数据获取成功');
      
    } catch (error) {
      console.error('获取数据时出错:', error);
      // 可以在这里添加错误处理逻辑，比如设置一个错误状态
    }
  }

  onMounted(() => getPublishs());
</script>

<template>
  <a-modal v-model:visible="open" title="发布寻物启事" >
    <template #footer></template>
    <a-form ref="formModel" :model="form" :labelCol="{ span: 5 }" :wrapperCol="{ span: 16 }" >
      <a-form-item label="物品名称" name="Item_Name" has-feedback :rules="[{ required: true, message: '请输入物品名称' }]">
        <a-input v-model:value="form.ITEM_Name" :maxlength="20" />
      </a-form-item>
      <a-form-item label="物品类别" name="Category_ID" has-feedback :rules="[{ required: true, message: '请选择物品类别' }]">
        <a-cascader v-model:value="form.CATEGORY_ID" :options="[{label: '物品类别1', value: '1',}, {label: '手表', value: '2',},]"/>
      </a-form-item>
      <a-form-item label="物品描述" name="Description" has-feedback :rules="[{ required: true, message: '请输入物品描述' }]">
        <a-textarea :rows="4" v-model:value="form.DESCRIPTION" :maxlength="100" />
      </a-form-item>
      <a-form-item label="丢失地点" name="Lost_Location" has-feedback :rules="[{ required: true, message: '请输入丢失地点' }]">
        <a-textarea :rows="4" v-model:value="form.LOST_LOCATION" :maxlength="100" />
      </a-form-item>
      <a-form-item label="丢失时间" name="Lost_Date" has-feedback :rules="[{ required: true, message: '请输入丢失时间' }]">
        <a-date-picker v-model:value="form.LOST_DATE" show-time/>
      </a-form-item>
      <a-form-item label="物品标签" name="Tag_ID" has-feedback :rules="[{ required: true, message: '请选择物品标签' }]">
        <a-radio-group v-model:value="form.TAG_ID">
          <a-radio value="1">贵重物品</a-radio>
          <a-radio value="2">私人用品</a-radio>
          <a-radio value="3">医疗用品</a-radio>
        </a-radio-group>
      </a-form-item>
      <a-form-item label="物品图片" name="Image_URL" has-feedback :rules="[{ required: true, message: '请上传物品图片' }]">
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
      <a-form-item label="是否悬赏" name="isRewarded">
        <a-switch v-model:checked="form.IS_REWARDED" />
      </a-form-item>
      <template v-if="form.IS_REWARDED">
        <a-form-item label="悬赏金额" name="Reward_Amount" has-feedback>
          <a-input-number v-model:value="form.REWARD_AMOUNT" prefix="￥" :min="1" :max="9999999999" />
        </a-form-item>
        <a-form-item label="悬赏截止时间" name="Deadline" has-feedback :rules="[{ required: true, message: '请输入悬赏截止时间' }]">
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
    <template #bodyCell="{ column, record }">
      <template v-if="column.dataIndex === 'itemNameAndCategory'">
        <div class="text-title font-bold">
          {{ record.ITEM_NAME }}
        </div>
        <div class="text-subtext">
          {{categoryMap[record.CATEGORY_ID]}}
        </div>
      </template>
      <template v-else-if="column.dataIndex === 'IMAGE_URL'">
        <img class="w-12 rounded" :src="record.IMAGE_URL" />
      </template>
      <template v-else-if="column.dataIndex === 'isRewarded'">
        <a-badge class="text-subtext" :color="'green'">
          <template #text>
            <span class="text-subtext">{{ IsRewardedDict[Number(record.isRewarded) as IsRewarded] }}</span>
          </template>
        </a-badge>
      </template>
      <template v-else-if="column.dataIndex === 'TAG_ID'">
        <span>
          <a-tag color="volcano">
              {{ tagMap[record.TAG_ID] }}
          </a-tag>
        </span>
      </template>
    </template>
  </a-table>
</template>
