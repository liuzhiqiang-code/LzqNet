import type { VbenFormSchema } from '#/adapter/form';
import { h } from 'vue';
import { z } from '#/adapter/form';
import { $t } from '#/locales';

export function useSchema(): VbenFormSchema[] {
  // 基本信息
  const basicSchema: VbenFormSchema[] = [
    {
      component: 'Input',
      fieldName: 'modelName',
      label: $t('modeling.model.modelName'),
      formItemClass: 'col-start-1',
      rules: z
        .string()
        .min(2, $t('ui.formRules.minLength', [$t('modeling.model.modelName'), 1]))
        .max(
          50,
          $t('ui.formRules.maxLength', [$t('modeling.model.modelName'), 50]),
        ),
    },
    {
      component: 'Textarea',
      componentProps: { autoSize: { minRows: 3, maxRows: 3 }, allowClear: true },
      fieldName: 'description',
      label: $t('modeling.model.description'),
      formItemClass: 'row-span-2 col-span-2',
    },
    {
      component: 'Checkbox',
      componentProps: { checked: true },
      fieldName: 'isEnable',
      renderComponentContent: () => {
        return {
          default: () => [$t('modeling.model.isEnable')],
        };
      }
    }
  ]

  // 扩展信息
  const extendSchema: VbenFormSchema[] = [
     {
      component: 'Divider',
      componentProps: { orientation: 'left' },
      fieldName: '_divider',
      formItemClass: 'col-span-3',
      hideLabel: true,
      renderComponentContent: () => {
        return {
          default: () => h('div', $t('modeling.model.extendSchema')),
        };
      },
    },
    {
      component: 'Input',
      fieldName: 'tableName',
      label: $t('modeling.model.tableName'),
      formItemClass: 'col-start-1',
      rules: z
        .string()
        .min(2, $t('ui.formRules.minLength', [$t('modeling.model.tableName'), 1]))
        .max(
          50,
          $t('ui.formRules.maxLength', [$t('modeling.model.tableName'), 50]),
        ),
    }
  ]

  // 审计信息
  const auditSchema: VbenFormSchema[] = [
    {
      component: 'Divider',
      componentProps: { orientation: 'left' },
      fieldName: '_divider',
      formItemClass: 'col-span-3',
      hideLabel: true,
      renderComponentContent: () => {
        return {
          default: () => h('div', $t('modeling.model.auditSchema')),
        };
      },
    },
    {
      component: 'Input',
      fieldName: 'creator',
      formItemClass: 'col-start-1',
      label: $t('modeling.model.creator'),
    },
    {
      component: 'DatePicker',
      componentProps: { showTime: true },
      fieldName: 'creationTime',
      formItemClass: 'col-start-2',
      label: $t('modeling.model.creationTime'),
    },
    {
      component: 'Input',
      fieldName: 'modifier',
      formItemClass: 'col-start-1',
      label: $t('modeling.model.modifier'),
    },
    {
      component: 'DatePicker',
      componentProps: { showTime: true },
      fieldName: 'modificationTime',
      formItemClass: 'col-start-2',
      label: $t('modeling.model.modificationTime'),
    },
  ]

  return [...basicSchema, ...extendSchema, ...auditSchema];
}
