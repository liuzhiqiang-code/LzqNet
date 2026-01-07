import { h, ref } from 'vue';
import { useDebounceFn } from '@vueuse/core';
import type { VbenFormSchema } from '#/adapter/form';
import { useFactorySchema } from './schemas/factory';
import { useWorkCenterSchema } from './schemas/workCenter';
import { Spin } from 'ant-design-vue';
import { $t } from '#/locales';

interface DataItem {
  name: string,
  title: string;
}
interface SelectItem {
  label: string,
  value: string;
}
const modelingKeyword = ref('');
const modelingLoading = ref(false);
// 远程获取数据
async function getModelingList({ keyword = '' }: Record<string, any>) {
  modelingLoading.value = true;
  await new Promise(resolve => setTimeout(resolve, 500))
  const data: DataItem[] = [
    {
      name: 'Factory',
      title: '工厂建模',
    },
    {
      name: 'WorkCenter',
      title: '车间建模',
    }
  ];
  const filterData = data.filter(item => item.title.includes(keyword) || item.name.includes(keyword));
  modelingLoading.value = false;
  return filterData.map(item => ({ label: item.name + '-' + item.title, value: item.name }));
}

const dataKeyword = ref('');
const dataLoading = ref(false);
async function getDataList({ modelingName, keyword = '' }: Record<string, any>) {
  dataLoading.value = true;
  await new Promise(resolve => setTimeout(resolve, 500))
  let data: SelectItem[] = []
  if (modelingName == 'Factory') {
    data = [
      {
        label: '1080',
        value: '1080',
      },
      {
        label: '1090',
        value: '1090',
      }
    ];
  }
  else if (modelingName == 'WorkCenter') {
    data = [
      {
        label: 'SE001',
        value: 'SE001',
      },
      {
        label: 'QA001',
        value: 'QA001',
      }
    ];
  }
  const options = data.filter(item => item.value.includes(keyword));
  dataLoading.value = false;
  return options
}

export function useSearchSchema(funcObj: Record<string, any>): VbenFormSchema[] {
  return [
    {
      component: 'ApiSelect',
      // 对应组件的参数
      componentProps: () => {
        return {
          api: getModelingList,
          onChange: funcObj.modelingNameOnChange,
          // 禁止本地过滤
          filterOption: false,
          // 如果正在获取数据，使用插槽显示一个loading
          notFoundContent: modelingLoading.value ? undefined : null,
          // 搜索词变化时记录下来， 使用useDebounceFn防抖。
          onSearch: useDebounceFn((value: string) => {
            modelingKeyword.value = value;
          }, 300),
          // 远程搜索参数。当搜索词变化时，params也会更新
          params: {
            keyword: modelingKeyword.value || undefined,
          },
          showSearch: true,
          //autoSelect: 'first',
        };
      },
      // 字段名
      fieldName: 'modelingName',
      // 界面显示的label
      label: $t('system.modeling.modelingName'),
      renderComponentContent: () => {
        return {
          notFoundContent: modelingLoading.value ? h(Spin) : undefined,
        };
      },
      rules: 'selectRequired',
    },
    {
      component: 'ApiSelect',
      // 字段名
      fieldName: 'dataName',
      // 界面显示的label
      label: $t('system.modeling.dataName'),
      renderComponentContent: () => {
        return {
          notFoundContent: dataLoading.value ? h(Spin) : undefined,
        };
      },
      rules: 'selectRequired',
      dependencies: {
        componentProps: (value) => {
          return {
            api: getDataList,
            onChange: funcObj.dataNameOnChange,
            // 禁止本地过滤
            filterOption: false,
            // 如果正在获取数据，使用插槽显示一个loading
            notFoundContent: dataLoading.value ? undefined : null,
            // 搜索词变化时记录下来， 使用useDebounceFn防抖。
            onSearch: useDebounceFn((value: string) => {
              dataKeyword.value = value;
            }, 300),
            // 远程搜索参数。当搜索词变化时，params也会更新
            params: {
              keyword: dataKeyword.value || undefined,
              modelingName: value.modelingName || undefined,
            },
            showSearch: true,
            autoSelect: 'first',
          };
        },
        // 只有指定的字段改变时，才会触发
        triggerFields: ['modelingName'],
      },
    },
  ]
}

/**
 * 获取编辑表单的字段配置。如果没有使用多语言，可以直接export一个数组常量
 */
export function useModelingSchema(modelingName: string): VbenFormSchema[] {
  if (modelingName == 'Factory') {
    return useFactorySchema()
  } else if (modelingName == 'WorkCenter') {
    return useWorkCenterSchema()
  }

  return []
}
