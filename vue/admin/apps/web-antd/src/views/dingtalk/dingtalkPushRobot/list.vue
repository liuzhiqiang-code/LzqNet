<script lang="ts" setup>
import type {
  OnActionClickParams,
  VxeTableGridOptions,
} from '#/adapter/vxe-table';
import { useVbenVxeGrid } from '#/adapter/vxe-table';
import { Page, useVbenModal } from '@vben/common-ui';
import { Plus } from '@vben/icons';
import { Button, message } from 'ant-design-vue';

import { $t } from '#/locales';

import type { DingtalkPushRobotApi } from '#/api/dingtalk/dingtalkPushRobot';
import { deleteDingtalkPushRobot, getDingtalkPushRobotList } from '#/api/dingtalk/dingtalkPushRobot';

import { useColumns } from './data';
import Form from './modules/form.vue';


const [FormModal, formModalApi] = useVbenModal({
  connectedComponent: Form,
  destroyOnClose: true,
});
/**
 * 编辑钉钉推送机器人
 * @param row
 */
function onEdit(row: DingtalkPushRobotApi.DingtalkPushRobot) {
  formModalApi.setData(row).open();
}
/**
 * 创建新钉钉推送机器人
 */
function onCreate() {
  formModalApi.setData(null).open();
}
/**
 * 删除钉钉推送机器人
 * @param row
 */
function onDelete(row: DingtalkPushRobotApi.DingtalkPushRobot) {
  const hideLoading = message.loading({
    content: $t('ui.actionMessage.deleting', [row.name]),
    duration: 0,
    key: 'action_process_msg',
  });
  deleteDingtalkPushRobot(row.id)
    .then(() => {
      message.success({
        content: $t('ui.actionMessage.deleteSuccess', [row.name]),
        key: 'action_process_msg',
      });
      refreshGrid();
    })
    .catch(() => {
      hideLoading();
    });
}
/**
 * 表格操作按钮的回调函数
 */
function onActionClick({
  code,
  row,
}: OnActionClickParams<DingtalkPushRobotApi.DingtalkPushRobot>) {
  switch (code) {
    case 'delete': {
      onDelete(row);
      break;
    }
    case 'edit': {
      onEdit(row);
      break;
    }
  }
}
const [Grid, gridApi] = useVbenVxeGrid({
  gridEvents: {},
  gridOptions: {
    columns: useColumns(onActionClick),
    height: 'auto',
    keepSource: true,
    pagerConfig: {
      enabled: false,
    },
    proxyConfig: {
      ajax: {
        query: async (_params) => {
          return await getDingtalkPushRobotList();
        },
      },
    },
    toolbarConfig: {
      custom: true,
      export: false,
      refresh: true,
      zoom: true,
    },
  } as VxeTableGridOptions,
});
/**
 * 刷新表格
 */
function refreshGrid() {
  gridApi.query();
}
</script>
<template>
  <Page auto-content-height>
    <FormModal @success="refreshGrid" />
    <Grid table-title="钉钉推送机器人列表">
      <template #toolbar-tools>
        <Button type="primary" @click="onCreate">
          <Plus class="size-5" />
          {{ $t('ui.actionTitle.create') }}
        </Button>
      </template>
    </Grid>
  </Page>
</template>