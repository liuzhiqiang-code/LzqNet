import type { VxeTableGridOptions } from '@vben/plugins/vxe-table';
import type { VbenFormSchema } from '#/adapter/form';
import type { OnActionClickFn } from '#/adapter/vxe-table';
import type { DingtalkPushRobotApi } from '#/api/dingtalk/dingtalkPushRobot';

import { z } from '#/adapter/form';
import { $t } from '#/locales';

/**
 * 获取编辑表单的字段配置。如果没有使用多语言，可以直接export一个数组常量
 */
export function useSchema(): VbenFormSchema[] {
  return [
    {
      component: 'Input',
      fieldName: 'name',
      label: $t('dingtalk.dingtalkPushRobot.name')
    },
    {
      component: 'Input',
      fieldName: 'dingtalkGroupName',
      label: $t('dingtalk.dingtalkPushRobot.dingtalkGroupName')
    },
    {
      component: 'Input',
      fieldName: 'enableStatus',
      label: $t('dingtalk.dingtalkPushRobot.enableStatus')
    },
    {
      component: 'Input',
      fieldName: 'webhook',
      label: $t('dingtalk.dingtalkPushRobot.webhook')
    },
    {
      component: 'Input',
      fieldName: 'pushKeywords',
      formItemClass: 'items-start',
      label: $t('dingtalk.dingtalkPushRobot.pushKeywords'),
      modelPropName: 'modelValue',
    },
    // {
    //   component: 'JsonViewer',
    //   fieldName: 'pushKeywords',
    //   label: $t('dingtalk.dingtalkPushRobot.pushKeywords')
    // },
    {
      component: 'Input',
      fieldName: 'sign',
      label: $t('dingtalk.dingtalkPushRobot.sign')
    },
    // {
    //   component: 'Input',
    //   fieldName: 'pushIpSegments',
    //   label: $t('dingtalk.dingtalkPushRobot.pushIpSegments')
    // },
         
  ];
}
/**
 * 获取表格列配置
 * @description 使用函数的形式返回列数据而不是直接export一个Array常量，是为了响应语言切换时重新翻译表头
 * @param onActionClick 表格操作按钮点击事件
 */
export function useColumns(
  onActionClick?: OnActionClickFn<DingtalkPushRobotApi.DingtalkPushRobot>,
): VxeTableGridOptions<DingtalkPushRobotApi.DingtalkPushRobot>['columns'] {
  return [
    {
      align: 'left',
      field: 'name',
      fixed: 'left',
      title: $t('dingtalk.dingtalkPushRobot.name')
    },
    {
      align: 'left',
      field: 'dingtalkGroupName',
      fixed: 'left',
      title: $t('dingtalk.dingtalkPushRobot.dingtalkGroupName')
    },
    {
      align: 'left',
      field: 'enableStatus',
      fixed: 'left',
      title: $t('dingtalk.dingtalkPushRobot.enableStatus')
    },
    {
      align: 'left',
      field: 'webhook',
      fixed: 'left',
      title: $t('dingtalk.dingtalkPushRobot.webhook')
    },
    {
      align: 'left',
      field: 'pushKeywords',
      fixed: 'left',
      title: $t('dingtalk.dingtalkPushRobot.pushKeywords')
    },
    {
      align: 'left',
      field: 'sign',
      fixed: 'left',
      title: $t('dingtalk.dingtalkPushRobot.sign')
    },
    {
      align: 'left',
      field: 'pushIpSegments',
      fixed: 'left',
      title: $t('dingtalk.dingtalkPushRobot.pushIpSegments')
    },
    
    // 操作按钮
    {
      align: 'right',
      cellRender: {
        attrs: {
          nameField: 'name',
          onClick: onActionClick,
        },
        name: 'CellOperation',
        options: [
          'edit', // 默认的编辑按钮
          {
            code: 'delete', // 默认的删除按钮
            disabled: (row: DingtalkPushRobotApi.DingtalkPushRobot) => {
              return !!(row.children && row.children.length > 0);
            },
          },
        ],
      },
      field: 'operation',
      fixed: 'right',
      headerAlign: 'center',
      showOverflow: false,
      title: $t('dingtalk.dingtalkPushRobot.operation'),
      width: 200,
    },
  ];
}