import type { VbenFormSchema } from '#/adapter/form';
import { h } from 'vue';
import { Checkbox,Textarea } from 'ant-design-vue';
import { z } from '#/adapter/form';
import { $t } from '#/locales';

export function useHeaderSchema() : VbenFormSchema[] {
  return [
    {
      component: 'Input',
      fieldName: 'instanceName',
      label: $t('system.modeling.instanceName'),
      formItemClass: 'col-span-1',
      rules: z
        .string()
        .min(2, $t('ui.formRules.minLength', [$t('system.modeling.instanceName'), 1]))
        .max(
          50,
          $t('ui.formRules.maxLength', [$t('system.modeling.instanceName'), 50]),
        ),
    },
    {
      component: h(Textarea, { autoSize: { minRows: 3, maxRows: 3 },allowClear:true }),
      fieldName: 'description',
      label: $t('system.modeling.description'),
      formItemClass:  'row-span-3 col-span-2',
      // rules: z
      //   .string()
      //   .min(2, $t('ui.formRules.minLength', [$t('system.modeling.description'), 0]))
      //   .max(
      //     500,
      //     $t('ui.formRules.maxLength', [$t('system.modeling.description'), 500]),
      //   ),
    },
    {
      component: h(Checkbox, { checked: true }),
      fieldName: 'isEnable',
      renderComponentContent: () => {
        return {
          default: () => [$t('system.modeling.isEnable')],
        };
      }
    },
    
  ];
}
