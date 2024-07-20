<script lang="ts" setup>
  import { ref } from 'vue';
  import axios from 'axios';
  import { onMounted } from 'vue';

  const columns = [
    { title: '丢失物品', dataIndex: 'itemNameAndCategory'},
    { title: '物品描述', dataIndex: 'itemDescribe', ellipsis: true},
    { title: '发现地点', dataIndex: 'findPosition', ellipsis: true},
    { title: '发现时间', dataIndex: 'findTime' },
    { title: '物品标签', dataIndex: 'itemTags' },
    { title: '物品图片', dataIndex: 'itemImage' },
  ];

  const finds = ref([])
  const getFinds = async () => {
    const res = await axios.get('api/finds')
    finds.value = res.data
  }
  onMounted(() => getFinds())
</script>

<template>
  <!-- 无主物品 -->
  <a-table :columns="columns" :dataSource="finds" >
    <template #title>
      <div class="flex justify-between pr-4">
        <h4>我发现的无主物品</h4>
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
