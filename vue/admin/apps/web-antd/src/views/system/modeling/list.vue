<script lang="ts" setup>
import { reactive, ref, onMounted, markRaw } from 'vue';
import { useVbenForm } from '#/adapter/form';
import { ColPage } from '@vben/common-ui';
import {
  Button,
  Card,
  Tabs,
  TabPane,
  message
} from 'ant-design-vue';
import { $t } from '#/locales';
import { useSchema } from './data';
import { useModelingSchema } from './modelingSchema'
import { ModelingApiObj } from '#/api/system/modeling';

const props = reactive({
  leftCollapsedWidth: 5,
  leftCollapsible: true,
  leftMaxWidth: 20,
  leftMinWidth: 15,
  leftWidth: 20,
  resizable: false,
  rightWidth: 80,
  splitHandle: false,
  splitLine: false,
});

onMounted(() => {
})

const getFormApi = ()=>{return searchFormApi}
const [SearchForm, searchFormApi] = useVbenForm({
  layout: 'vertical',
  schema:useSchema(getFormApi),
  showDefaultActions: false,
  wrapperClass: 'grid-cols-1',
  // 所有表单项共用，可单独在表单内覆盖
  commonConfig: {
    // 所有表单项
    componentProps: {
      class: 'w-full',
    },
  },
});

async function handleAdd() { 
  const searchValues = await searchFormApi.getValues()
  if (!searchValues.modelingName) {
    message.error('需要先选择建模名称')
    return
  }
  await addTab({
      modelingName:searchValues.modelingName, 
      modelingDesc:searchValues.modelingDesc,
      dataId:'',
      dataName:'',
    })
}
async function handleCopy() { 
  const activeTab = panes.value.find(pane => pane.key === activeKey.value)
  if(activeTab)
    await addTab({
      modelingName:activeTab.modelingName,
      modelingDesc:activeTab.modelingDesc,
      dataId:activeTab.dataId,
      dataName:activeTab.dataName,
      isCopy:true
    })
}
function handleReset() { 
  const activeTab = panes.value.find(pane => pane.key === activeKey.value)
  if(activeTab)
    activeTab.formApi.resetForm()
}
function handleSave() { }

interface TabItem{
  title:string,
  key?: string,
  modelingName: string,
  modelingDesc:string,
  dataId: string,
  dataName:string,
  component: any,
  formApi: any
}
interface AddTabDto{
  modelingName: string,
  modelingDesc:string,
  dataId: string,
  dataName: string,
  isCopy?: boolean
}
const panes = ref<TabItem[]>([]);
const activeKey = ref('');
const handleOpenTab = async ()=>{
  const searchValues = await searchFormApi.getValues()
  if (!searchValues.modelingName || !searchValues.dataId) {
    message.error('请选择建模名称和数据名称')
    return
  }
  await addTab({
    modelingName:searchValues.modelingName,
    modelingDesc:searchValues.modelingDesc,
    dataId:searchValues.dataId,
    dataName:searchValues.dataName
  })
}
const addTab = async ({modelingName,modelingDesc,dataId,dataName,isCopy = false}:AddTabDto)=>{
  var key = modelingDesc + '-' + dataName
  if(isCopy)
    key += '-copy'
  activeKey.value = key

  // 检查是否已存在相同 key 的标签页
  const existingTab = panes.value.find(pane => pane.key === key)
  if (existingTab) {
    // 如果已存在，直接跳转到该标签页
    activeKey.value = key
    return
  }

  const [Form, formApi] = useVbenForm({
    layout: 'vertical',
    schema: await useModelingSchema(modelingName),
    showDefaultActions: false,
    wrapperClass: 'grid-cols-1 md:grid-cols-2 lg:grid-cols-3',
    commonConfig: {
      componentProps: {
        class: 'w-full',
      },
    },
  });
  const tab : TabItem = {
    title: key,
    key: key, 
    modelingName,
    modelingDesc,
    dataId,
    dataName,
    component: markRaw(Form),
    formApi: formApi 
  }
  panes.value.push(tab);
  if(dataId)
  {
    const data = await getSelectDataValue(tab)
    if(data)
    {
      if(isCopy)
        data.instanceName += '-copy'
      formApi.setValues(data)
    }
  }
}
const getSelectDataValue = async(tab: TabItem) => {
  const data = await ModelingApiObj.getData({modelingName:tab.modelingName,dataId:tab.dataId})
  return data
}

const remove = (targetKey: string) => {
  let lastIndex = 0;
  panes.value.forEach((pane, i) => {
    if (pane.key === targetKey) {
      lastIndex = i - 1;
    }
  });
  panes.value = panes.value.filter(pane => pane.key !== targetKey);
  if (panes.value.length && activeKey.value === targetKey) {
    if (lastIndex >= 0) {
      activeKey.value = panes.value[lastIndex]!.key!;
    } else {
      activeKey.value = panes.value[0]!.key!;
    }
  }
};

type Key = /*unresolved*/ any
const onEdit = (targetKey: MouseEvent | Key | KeyboardEvent, action: "add" | "remove") => {
  if (action === 'add') {
    return
  } else {
    remove(targetKey as string);
  }
};

</script>
<template>
  <ColPage auto-content-height v-bind="props">
    <template #left>
      <div :style="{ minWidth: '160px' }" class="mr-2 rounded-[var(--radius)] border border-border bg-card p-2">
        <SearchForm class="mx-4" />
          <div class="flex justify-center mt-2">
            <Button class="mr-2" @click="handleOpenTab">{{ $t('system.modeling.btnOpenTab')}}</Button>
          </div>
      </div>
    </template>
    <Card class="ml-2">
      <template #extra>
        <Button class="mr-2" @click="handleAdd">{{ $t('system.modeling.btnAdd')}}</Button>
        <Button class="mr-2" @click="handleCopy">{{ $t('system.modeling.btnCopy')}}</Button>
        <Button class="mr-2" @click="handleReset">{{ $t('system.modeling.btnReset')}}</Button>
        <Button @click="handleSave">{{ $t('system.modeling.btnSave')}}</Button>
      </template>

      <Tabs v-model:activeKey="activeKey" hide-add type="editable-card" @edit="onEdit">
        <TabPane v-for="pane in panes" :key="pane.key" :tab="pane.title">
          <component 
            :is="panes.find(t => t.key === pane.key)?.component" 
            class="mx-4" 
          />
        </TabPane>
      </Tabs>

      <!-- <ModelingForm class="mx-4" /> -->
    </Card>
  </ColPage>
</template>
