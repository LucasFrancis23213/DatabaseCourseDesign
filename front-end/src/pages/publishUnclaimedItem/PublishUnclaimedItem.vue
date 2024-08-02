<script lang="ts" setup>
  import { getBase64 } from '@/utils/file';
  import { ref } from 'vue';
  import { FormInstance } from 'ant-design-vue';
  import { message } from 'ant-design-vue';
  import axios from 'axios';
  import { onMounted } from 'vue';
  import dayjs from 'dayjs';

  const baseURL = 'http://121.36.200.128:5000/api/';

  type oneFind = {
    itemName?: string;
    itemCategory?: string;
    itemDescribe?: string;
    findPosition?: string;
    findTime?: string;
    itemTags?:string[];
    itemImage?: string;
  };
  const formModel = ref<FormInstance>();
  const submit = () => {
    loading.value = true;
    formModel.value?.validateFields()
      .then(async ()=>{
        if (selectedFile.value) {
          const formData = new FormData();
          formData.append('file', selectedFile.value);
          const res = await axios.post(baseURL + 'ItemPicUpload/upload?type=Found', formData, {
            headers: {
              'Content-Type': 'multipart/form-data',
            },
          });
          form.value.itemImage = res.data.url;
        }
        
        form.value.itemCategory = form.value.itemCategory[0]
        form.value.findTime = dayjs(form.value.findTime).format("YYYY-MM-DD HH:mm:ss")
        const jsonFormData = JSON.stringify(form.value)
        console.log(jsonFormData)
        await axios.post('api/addNewFind', jsonFormData)
        getFinds()
        setTimeout(() => {
          message.success('提交成功！');
          loading.value = false;
          open.value = false;
        }, 2000);
      })
      .catch(()=>{
        loading.value = false;
        message.error('提交失败！')
      })
  };
  const columns = [
    { title: '丢失物品', dataIndex: 'itemNameAndCategory'},
    { title: '物品描述', dataIndex: 'itemDescribe', ellipsis: true},
    { title: '发现地点', dataIndex: 'findPosition', ellipsis: true},
    { title: '发现时间', dataIndex: 'findTime' },
    { title: '物品标签', dataIndex: 'itemTags' },
    { title: '物品图片', dataIndex: 'itemImage' },
  ];
  const open = ref<boolean>(false);
  const loading = ref<boolean>(false);
  const addNew = () => {
    open.value = true;
  }
  const form = ref<oneFind>({
    itemName : '',
    itemCategory : '',
    itemDescribe : '',
    findPosition : '',
    findTime : '',
    itemTags : [],
    itemImage : '',
  });
  const selectedFile = ref<File | null>(null);
  const handleFileChange = (file : File) => {
    selectedFile.value = file;
    const reader = new FileReader();
    reader.onload = (e) => {
      form.value.itemImage = e.target?.result as string;
    };
    reader.readAsDataURL(file);
  };
  async function extractImg(file: File) {
    handleFileChange(file);
  };

  const finds = ref([])
  const getFinds = async () => {
    const res = await axios.get('api/finds')
    finds.value = res.data
  }
  onMounted(() => getFinds())
</script>

<template>
  <a-modal v-model:visible="open" title="发布无主物品" >
    <template #footer></template>
    <a-form ref="formModel" :model="form" :labelCol="{ span: 5 }" :wrapperCol="{ span: 16 }" >
      <a-form-item label="物品名称" name="itemName" has-feedback :rules="[{ required: true, message: '请输入物品名称' }]">
        <a-input v-model:value="form.itemName" :maxlength="20" />
      </a-form-item>
      <a-form-item label="物品类别" name="itemCategory" has-feedback :rules="[{ required: true, message: '请选择物品类别' }]">
        <a-cascader v-model:value="form.itemCategory" :options="[{label: '物品类别1', value: '物品类别1',}, {label: '手表', value: '手表',},]"/>
      </a-form-item>
      <a-form-item label="物品描述" name="itemDescribe" has-feedback :rules="[{ required: true, message: '请输入物品描述' }]">
        <a-textarea :rows="4" v-model:value="form.itemDescribe" :maxlength="100" />
      </a-form-item>
      <a-form-item label="发现地点" name="findPosition" has-feedback :rules="[{ required: true, message: '请输入发现地点' }]">
        <a-textarea :rows="4" v-model:value="form.findPosition" :maxlength="100" />
      </a-form-item>
      <a-form-item label="发现时间" name="findTime" has-feedback :rules="[{ required: true, message: '请输入发现时间' }]">
        <a-date-picker v-model:value="form.findTime" show-time/>
      </a-form-item>
      <a-form-item label="物品标签" name="itemTags" has-feedback :rules="[{ required: true, message: '请选择物品标签' }]">
      <a-checkbox-group v-model:value="form.itemTags">
        <a-checkbox value="贵重物品" >贵重物品</a-checkbox>
        <a-checkbox value="私人用品" >私人用品</a-checkbox>
        <a-checkbox value="医疗用品" >医疗用品</a-checkbox>
      </a-checkbox-group>
    </a-form-item>
      <a-form-item label="物品图片" name="itemImage" has-feedback :rules="[{ required: true, message: '请上传物品图片' }]">
        <a-upload :show-upload-list="false" :beforeUpload="(file: File) => extractImg(file)">
          <img class="h-8 p-0.5 rounded border border-dashed border-border" v-if="form.itemImage" :src="form.itemImage" />
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
 
  <!-- 无主物品 -->
  <a-table :columns="columns" :dataSource="finds" >
    <template #title>
      <div class="flex justify-between pr-4">
        <h4>无主物品</h4>
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
          {{ record.itemName }}
        </div>
        <div class="text-subtext">
          {{record.itemCategory}}
        </div>
      </template>
      <template v-else-if="column.dataIndex === 'itemImage'">
        <img class="w-12 rounded" :src="record.itemImage" />
      </template>
      <template v-else-if="column.dataIndex === 'itemTags'">
        <span>
          <a-tag
            v-for="tag in record.itemTags"
            :key="tag"
            :color="tag === '贵重物品' ? 'volcano' : tag.length > 4 ? 'geekblue' : 'green'"
            >
              {{ tag.toUpperCase() }}
          </a-tag>
        </span>
      </template>
    </template>
  </a-table>
</template>
