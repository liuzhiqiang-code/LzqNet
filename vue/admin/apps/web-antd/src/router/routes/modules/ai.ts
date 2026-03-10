import type { RouteRecordRaw } from 'vue-router';

import { $t } from '#/locales';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'ion:settings-outline',
      order: 9997,
      title: $t('ai.title'),
    },
    name: 'ai',
    path: '/ai',
    children: [
      {
        path: '/ai/aiChats',
        name: 'aiChats',
        meta: {
          icon: 'mdi:account-group',
          title: $t('ai.aiChats.title'),
        },
        component: () => import('#/views/ai/aiChats/index.vue'),
      }
    ],
  },
];

export default routes;
