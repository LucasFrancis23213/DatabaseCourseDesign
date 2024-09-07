import {computed} from "vue";
const categoryMapping = {
    '1': '日用品',
    '2': '手表',
    '3': '电子产品',
    '4': '办公用品',
    '5': '交通用具',
    '6': '玩具',
  };
const categoryOptions = computed(() =>
      Object.entries(categoryMapping).map(([value, label]) => ({
        label,
        value,
      }))
    );

export default {categoryMapping,categoryOptions};

