import { h, ref } from 'vue';
import { useDebounceFn } from '@vueuse/core';
import type { VbenFormSchema, ExtendedFormApi } from '#/adapter/form';
import { Spin } from 'ant-design-vue';
import { $t } from '#/locales';
import { getModelingSelectList, ModelingApiObj } from '#/api/system/modeling';
import type { SelectViewDto } from '#/api/types';

const modelingKeyword = ref('');
const modelingLoading = ref(false);
// 远程获取数据
async function getModelingList({ keyword = '' }: Record<string, any>) {
  modelingLoading.value = true;
  const data = await getModelingSelectList({ keyword })
  modelingLoading.value = false;
  return data;
}

const dataKeyword = ref('');
const dataLoading = ref(false);
async function getDataList({ modelingName, keyword = '' }: Record<string, any>) {
  if (!modelingName)
    return []
  dataLoading.value = true;
  await new Promise(resolve => setTimeout(resolve, 500))
  const data = await ModelingApiObj.getSelectList({ modelingName, keyword })
  dataLoading.value = false;
  return data
}

export function useSchema(getFormApi: () => ExtendedFormApi): VbenFormSchema[] {
  return [
    {
      component: 'ApiSelect',
      // 对应组件的参数
      componentProps: () => {
        return {
          api: getModelingList,
          onChange: (value: string, data: SelectViewDto) => {
            const formApi = getFormApi()
            formApi.setFieldValue("dataName", null)
            formApi.setFieldValue("modelingDesc", data.label)
          },
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
      fieldName: 'dataId',
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
            onChange: (value: string, data: SelectViewDto) => {
              const formApi = getFormApi()
              formApi.setFieldValue("dataName", data.label)
            },
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
            //autoSelect: 'first',
          };
        },
        // 只有指定的字段改变时，才会触发
        triggerFields: ['modelingName'],
      },
    },
  ]
}
