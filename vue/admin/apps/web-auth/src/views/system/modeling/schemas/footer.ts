import type { VbenFormSchema } from '#/adapter/form';
import { h } from 'vue';
import {Input, Checkbox,Textarea,Divider,DatePicker } from 'ant-design-vue';
import { z } from '#/adapter/form';
import { $t } from '#/locales';

export function useFooterSchema() : VbenFormSchema[] {
  return [
    {
      component: h(Divider,{orientation:'left'}),
      fieldName: '_divider',
      formItemClass: 'col-span-3',
      hideLabel: true,
      renderComponentContent: () => {
        return {
          default: () => h('div', $t('system.modeling.auditInfo')),
        };
      },
    },
    {
      component: h(Input),
      fieldName: 'creator',
      formItemClass: 'col-start-1',
      label: '创建人',
    },
    {
      component: h(DatePicker,{showTime:true}),
      fieldName: 'creationTime',
      formItemClass: 'col-start-2',
      label: '创建时间',
    },
    {
      component: h(Input),
      fieldName: 'modifier',
      formItemClass: 'col-start-1',
      label: '最后修改人',
    },
    {
      component: h(DatePicker,{showTime:true}),
      fieldName: 'modificationTime',
      formItemClass: 'col-start-2',
      label: '最后修改时间',
    },
  ];
}
