<script lang="ts" setup>
  import { getBase64 } from '@/utils/file';
  import { ref } from 'vue';
  import { FormInstance } from 'ant-design-vue';
  import { message } from 'ant-design-vue';
  import axios from 'axios';
  import { onMounted } from 'vue';

  type onePublish = {
    itemName?: string;
    itemCategory?: string;
    itemDescribe?: string;
    lostPosition?: string;
    lostTime?: string;
    itemTags?:string[];
    itemImage?: string;
    isRewarded?: boolean;
    rewardAmount?: string;
    deadlineTime?: string;
  };
  const formModel = ref<FormInstance>();
  const submit = () => {
    loading.value = true;
    formModel.value?.validateFields()
      .then(async ()=>{
        form.value.itemCategory = form.value.itemCategory[0]
        await axios.post('api/addNewPublish', form.value)
        getPublishs()
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
    { title: '遗失地点', dataIndex: 'lostPosition', ellipsis: true},
    { title: '丢失时间', dataIndex: 'lostTime' },
    { title: '物品标签', dataIndex: 'itemTags' },
    { title: '物品图片', dataIndex: 'itemImage' },
    { title: '是否悬赏', dataIndex: 'isRewarded' },
    { title: '悬赏金额', dataIndex: 'rewardAmount' },
    { title: '截止时间', dataIndex: 'deadlineTime' },
  ];
  const open = ref<boolean>(false);
  const loading = ref<boolean>(false);
  const addNew = () => {
    open.value = true;
  }
  const form = ref<onePublish>({
    itemName : '',
    itemCategory : '',
    itemDescribe : '',
    lostPosition : '',
    lostTime : '',
    itemTags : [],
    itemImage : '',
    isRewarded : false,
    rewardAmount : '1',
    deadlineTime: '',
  });
  async function extractImg(file: Blob, publish: onePublish) {
    await getBase64(file).then((res) => {
      publish.itemImage = res;
    });
  }
  type IsRewarded = 0 | 1;
  const IsRewardedDict = {
    0: '未悬赏',
    1: '悬赏',
  };
  const publishs = ref([])
  const getPublishs = async () => {
    const res = await axios.get('api/publishs')
    publishs.value = res.data
  }
  onMounted(() => getPublishs())
</script>

<template>
  <a-modal v-model:visible="open" title="发布寻物启事" >
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
      <a-form-item label="丢失地点" name="lostPosition" has-feedback :rules="[{ required: true, message: '请输入丢失地点' }]">
        <a-textarea :rows="4" v-model:value="form.lostPosition" :maxlength="100" />
      </a-form-item>
      <a-form-item label="丢失时间" name="lostTime" has-feedback :rules="[{ required: true, message: '请输入丢失时间' }]">
        <a-date-picker v-model:value="form.lostTime" show-time/>
      </a-form-item>
      <a-form-item label="物品标签" name="itemTags" has-feedback :rules="[{ required: true, message: '请选择物品标签' }]">
      <a-checkbox-group v-model:value="form.itemTags">
        <a-checkbox value="贵重物品" >贵重物品</a-checkbox>
        <a-checkbox value="私人用品" >私人用品</a-checkbox>
        <a-checkbox value="医疗用品" >医疗用品</a-checkbox>
      </a-checkbox-group>
    </a-form-item>
      <a-form-item label="物品图片" name="itemImage" has-feedback :rules="[{ required: true, message: '请上传物品图片' }]">
        <a-upload :show-upload-list="false" :beforeUpload="(file: File) => extractImg(file, form)">
          <img class="h-8 p-0.5 rounded border border-dashed border-border" v-if="form.itemImage" :src="form.itemImage" />
          <a-button v-else type="dashed">
            <template #icon>
              <UploadOutlined />
            </template>
            上传
          </a-button>
        </a-upload>
      </a-form-item>
      <a-form-item label="是否悬赏" name="isRewarded" has-feedback :rules="[{ required: true, message: '请选择是否悬赏' }]">
        <a-switch v-model:checked="form.isRewarded" />
      </a-form-item>
      <template v-if="form.isRewarded">
        <a-form-item label="悬赏金额" name="rewardAmount" has-feedback>
          <a-input-number v-model:value="form.rewardAmount" prefix="￥" :min="1" :max="9999999999" />
        </a-form-item>
        <a-form-item label="悬赏截止时间" name="deadlineTime" has-feedback :rules="[{ required: true, message: '请输入悬赏截止时间' }]">
          <a-date-picker v-model:value="form.deadlineTime" show-time/>
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
          {{ record.itemName }}
        </div>
        <div class="text-subtext">
          {{record.itemCategory}}
        </div>
      </template>
      <template v-else-if="column.dataIndex === 'itemImage'">
        <img class="w-12 rounded" :src="record.itemImage" />
      </template>
      <template v-else-if="column.dataIndex === 'isRewarded'">
        <a-badge class="text-subtext" :color="'green'">
          <template #text>
            <span class="text-subtext">{{ IsRewardedDict[Number(record.isRewarded) as IsRewarded] }}</span>
          </template>
        </a-badge>
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