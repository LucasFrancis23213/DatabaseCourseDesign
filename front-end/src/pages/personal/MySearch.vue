<script lang="ts" setup>
  import { ref, onMounted } from 'vue';
  import axios from 'axios';
  import { message } from 'ant-design-vue';
  import { useAccountStore } from '@/store/account';
  const {account, permissions} = useAccountStore();
  import ItemMap from "@/pages/admin_BasicFeature/ItemMap";
  const { categoryMapping } = ItemMap;

  axios.defaults.baseURL = import.meta.env.VITE_API_URL;

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
    { title: '删除', dataIndex: 'delete'},
  ];

  type IsRewarded = 0 | 1;
  const IsRewardedDict = {
    0: '未悬赏',
    1: '悬赏',
  };

  const tagMapping = {
  1: '贵重物品',
  2: '私人用品',
  3: '医疗用品'
};


  const publishs = ref([]);
  const myItems = ref([]);

  const getPublishs = async () => {
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

    for (var publish in publishs.value) {
      if (publishs.value[publish].USER_ID == account.userId) {
        myItems.value.push(publishs.value[publish]);
      }
    }

      
      console.log('数据获取成功');
      
    } catch (error) {
      console.error('获取数据时出错:', error);
      // 可以在这里添加错误处理逻辑，比如设置一个错误状态
    }
  }

  onMounted(() => getPublishs());

  // TODO: 删除功能，获取当前行的物品id，通过物品id调用删除接口，更新最新的列表
  const iconLoading = ref<boolean>(false);
  const deleteOneLine = async(row) => {
    iconLoading.value = true;
    try {
        // 调用接口删除founditem中的信息，使用DELETE方法
        await axios.delete('api/DeleteItem', {
            data: JSON.stringify({ 
                ITEM_ID: row,
                type: 0
            }),
            headers: {
                'Content-Type': 'application/json'
            }
        });
        getPublishs()
        alert("已删除！")
        // 向用户发送邮件通知
        // 注意：发送邮件的逻辑可能需要在后端实现
    } catch (error) {
        console.error('删除失败:', error);
        alert("删除失败，请重试！")
    }
  }
</script>

<template>
  <!-- 寻物启事 -->
  <a-table :columns="columns" :dataSource="myItems" >
    <template #title>
      <div class="flex justify-between pr-4">
        <a-button type="primary"><h4>我发布的寻物启事</h4> </a-button>
      </div>
    </template>
    <template #bodyCell="{ column, record }">
      <template v-if="column.dataIndex === 'itemNameAndCategory'">
        <div class="text-title font-bold">
          {{ record.ITEM_NAME }}
        </div>
        <div class="text-subtext">
          {{record.CATEGORY_ID}}
        </div>
      </template>
      <template v-else-if="column.dataIndex === 'IMAGE_URL'">
        <a-image class="w-12 rounded" :src="record.IMAGE_URL" />
      </template>
      <template v-else-if="column.dataIndex === 'IS_REWARDED'">
        <a-badge class="text-subtext" :color="'green'">
          <template #text>
            <span class="text-subtext">{{ IsRewardedDict[Number(record.IS_REWARDED) as IsRewarded] }}</span>
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
              {{ tag.toUpperCase() }}
          </a-tag>
        </span>
      </template>
      <template v-else-if="column.dataIndex === 'REWARD_AMOUNT'">
        <span>
          ￥{{ record.REWARD_AMOUNT }}
        </span>
      </template>
      <template v-else-if="column.dataIndex === 'delete'">
        <a-button type="primary" danger :loading="iconLoading" @click="deleteOneLine(record.ITEM_ID)">
            <DeleteOutlined />
            删除此条
        </a-button>
      </template>
    </template>
  </a-table>
</template>
