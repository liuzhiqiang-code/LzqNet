<script lang="ts" setup>
import { computed, ref } from 'vue';
import { useVbenModal } from '@vben/common-ui';
import { Button, List, ListItem,Input } from 'ant-design-vue';
import { useVbenForm } from '#/adapter/form';
import { $t } from '#/locales';

import type { DingtalkPushRobotApi } from '#/api/dingtalk/dingtalkPushRobot';
import { createDingtalkPushRobot, updateDingtalkPushRobot } from '#/api/dingtalk/dingtalkPushRobot';

import { useSchema } from '../data';


const emit = defineEmits(['success']);
const formData = ref<DingtalkPushRobotApi.DingtalkPushRobot>();
const getTitle = computed(() => {
  return formData.value?.id
    ? $t('ui.actionTitle.edit', [$t('dingtalk.dingtalkPushRobot.name')])
    : $t('ui.actionTitle.create', [$t('dingtalk.dingtalkPushRobot.name')]);
});

const [Form, formApi] = useVbenForm({
  layout: 'vertical',
  schema: useSchema(),
  showDefaultActions: false,
});

function resetForm() {
  formApi.resetForm();
  formApi.setValues(formData.value || {});
}

const [Modal, modalApi] = useVbenModal({
  async onConfirm() {
    const { valid } = await formApi.validate();
    if (valid) {
      modalApi.lock();
      const data = await formApi.getValues();
      try {
        await (formData.value?.id
          ? updateDingtalkPushRobot({ ...data, id: formData.value.id } as DingtalkPushRobotApi.DingtalkPushRobot)
          : createDingtalkPushRobot(data as DingtalkPushRobotApi.DingtalkPushRobot));
        modalApi.close();
        emit('success');
      } finally {
        modalApi.lock(false);
      }
    }
  },
  onOpenChange(isOpen) {
    if (isOpen) {
      const data = modalApi.getData<DingtalkPushRobotApi.DingtalkPushRobot>();
      if (data) {
        if (data.pid === 0) {
          data.pid = undefined;
        }
        formData.value = data;
        formApi.setValues(formData.value);
      }
    }
  },
});


const handleChange = async (index :any, e :any) => {
  let formValue = await formApi.getValues()
  formValue.pushKeywords[index] = e.target.value
  formApi.setFieldValue("pushKeywords",formValue.pushKeywords)
}
const addPushKeywords = async () =>{
  formData.value?.pushKeywords.push('')
  formApi.setFieldValue('pushKeywords',formData.value?.pushKeywords)
}
const updateItem = (index : number, value :any) => {
  let data = formData.value?.pushKeywords
  data![index] = value
  formApi.setFieldValue('pushKeywords',data)
}

const getItems = computed(() => {
  return formData.value?.pushKeywords
});
</script>

<template>
  <Modal :title="getTitle">
    <Form class="mx-4">
      <template #pushKeywords="slotProps">
        <!-- {{ JSON.stringify(slotProps) }} -->
          <div v-for="(item, index) in formData?.pushKeywords" :key="index" class="list-item">
            <Input 
        :value="item" 
        @update:value="(val) => updateItem(index, val)"
        placeholder="请输入内容"
      />
      <!-- <Button
        @click="removeItem(index)" 
        type="link" 
        danger
        style="margin-left: 8px"
      >
        删除
      </Button> -->
    </div>
    
    <Button @click="addPushKeywords" type="dashed" block style="margin-top: 8px">
      + 添加新项
    </Button>

        <!-- <Button type="primary" danger @click="addPushKeywords"> 新增 </Button>
        <List bordered :data-source="slotProps.modelValue">
          <template #renderItem="{ item ,index}">
            <ListItem>
              <Input :value="item" 
                @change="(e) => handleChange(index, e)"  />
            </ListItem>
          </template>
        </List> -->
      </template>
    </Form>
    <template #prepend-footer>
      <div class="flex-auto">
        <Button type="primary" danger @click="resetForm">
          {{ $t('common.reset') }}
        </Button>
      </div>
    </template>
  </Modal>
</template>