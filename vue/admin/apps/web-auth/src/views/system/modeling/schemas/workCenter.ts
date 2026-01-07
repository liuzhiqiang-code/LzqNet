import type { VbenFormSchema } from '#/adapter/form';
import { useFooterSchema } from './footer';
import { useHeaderSchema } from './header';
import { h } from 'vue';
import { z } from '#/adapter/form';
import { $t } from '#/locales';
import { getAllMenusApi } from '#/api';

export function useWorkCenterSchema(): VbenFormSchema[] {
  let schema: VbenFormSchema[] = [
    {
      component: 'Divider',
      componentProps: { orientation: 'left' },
      fieldName: '_divider',
      formItemClass: 'col-span-3',
      hideLabel: true,
      renderComponentContent: () => {
        return {
          default: () => h('div', $t('system.modeling.basicInfo')),
        };
      },
    },
    {
      component: 'ApiSelect',
      fieldName: 'api',
      label: 'ApiSelect',
      componentProps: {
        // 接口转options格式
        afterFetch: (data: { name: string; path: string }[]) => {
          return data.map((item: any) => ({
            label: item.name,
            value: item.path,
          }));
        },
        api: getAllMenusApi,
        autoSelect: 'first',
      },
    },
  ]

  return [...useHeaderSchema(), ...schema, ...useFooterSchema()];
}
