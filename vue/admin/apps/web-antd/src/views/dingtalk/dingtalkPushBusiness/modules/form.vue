<script lang="ts" setup>
import { computed, ref } from 'vue';
import { useVbenModal } from '@vben/common-ui';
import { Button } from 'ant-design-vue';
import { useVbenForm } from '#/adapter/form';
import { $t } from '#/locales';

import type { DingtalkPushBusinessApi } from '#/api/dingtalk/dingtalkPushBusiness';
import { createDingtalkPushBusiness, updateDingtalkPushBusiness } from '#/api/dingtalk/dingtalkPushBusiness';

import { useSchema } from '../data';


const emit = defineEmits(['success']);
const formData = ref<DingtalkPushBusinessApi.DingtalkPushBusiness>();
const getTitle = computed(() => {
  return formData.value?.id
    ? $t('ui.actionTitle.edit', [$t('dingtalk.dingtalkPushBusiness.name')])
    : $t('ui.actionTitle.create', [$t('dingtalk.dingtalkPushBusiness.name')]);
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
          ? updateDingtalkPushBusiness({ ...data, id: formData.value.id } as DingtalkPushBusinessApi.DingtalkPushBusiness)
          : createDingtalkPushBusiness(data as DingtalkPushBusinessApi.DingtalkPushBusiness));
        modalApi.close();
        emit('success');
      } finally {
        modalApi.lock(false);
      }
    }
  },
  onOpenChange(isOpen) {
    if (isOpen) {
      const data = modalApi.getData<DingtalkPushBusinessApi.DingtalkPushBusiness>();
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
</script>

<template>
  <Modal :title="getTitle">
    <Form class="mx-4" />
    <template #prepend-footer>
      <div class="flex-auto">
        <Button type="primary" danger @click="resetForm">
          {{ $t('common.reset') }}
        </Button>
      </div>
    </template>
  </Modal>
</template>
