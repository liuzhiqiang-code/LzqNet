import type { VxeTableGridOptions } from '@vben/plugins/vxe-table';
import type { VbenFormSchema } from '#/adapter/form';
import type { OnActionClickFn } from '#/adapter/vxe-table';
import type { DingtalkPushBusinessApi } from '#/api/dingtalk/dingtalkPushBusiness';

import { z } from '#/adapter/form';
import { $t } from '#/locales';

/**
 * 获取编辑表单的字段配置。如果没有使用多语言，可以直接export一个数组常量
 */
export function useSchema(): VbenFormSchema[] {
  return [
    {
      component: 'Input',
      fieldName: 'businessName',
      label: $t('dingtalk.dingtalkPushBusiness.businessName')
    },
    {
      component: 'Input',
      fieldName: 'enableStatus',
      label: $t('dingtalk.dingtalkPushBusiness.enableStatus')
    },
         
  ];
}
/**
 * 获取表格列配置
 * @description 使用函数的形式返回列数据而不是直接export一个Array常量，是为了响应语言切换时重新翻译表头
 * @param onActionClick 表格操作按钮点击事件
 */
export function useColumns(
  onActionClick?: OnActionClickFn<DingtalkPushBusinessApi.DingtalkPushBusiness>,
): VxeTableGridOptions<DingtalkPushBusinessApi.DingtalkPushBusiness>['columns'] {
  return [
    {
      align: 'left',
      field: 'businessName',
      fixed: 'left',
      title: $t('dingtalk.dingtalkPushBusiness.businessName')
    },
    {
      align: 'left',
      field: 'enableStatus',
      fixed: 'left',
      title: $t('dingtalk.dingtalkPushBusiness.enableStatus')
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
            disabled: (row: DingtalkPushBusinessApi.DingtalkPushBusiness) => {
              return !!(row.children && row.children.length > 0);
            },
          },
        ],
      },
      field: 'operation',
      fixed: 'right',
      headerAlign: 'center',
      showOverflow: false,
      title: $t('dingtalk.dingtalkPushBusiness.operation'),
      width: 200,
    },
  ];
}
