import type { RouteRecordRaw } from 'vue-router';

import { $t } from '#/locales';

const routes: RouteRecordRaw[] = [
  {
    meta: {
      icon: 'ion:settings-outline',
      order: 9997,
      title: $t('dingtalk.title'),
    },
    name: 'Dingtalk',
    path: '/dingtalk',
    children: [
      {
        path: '/dingtalk/dingtalkPushRobot',
        name: 'DingtalkPushRobot',
        meta: {
          icon: 'mdi:account-group',
          title: $t('dingtalk.dingtalkPushRobot.title'),
        },
        component: () => import('#/views/dingtalk/dingtalkPushRobot/list.vue'),
      },
      {
        path: '/dingtalk/dingtalkpushbusiness',
        name: 'Dingtalkpushbusiness',
        meta: {
          icon: 'mdi:account-group',
          title: $t('dingtalk.dingtalkpushbusiness.title'),
        },
        component: () => import('#/views/dingtalk/dingtalkpushbusiness/list.vue'),
      }
    ],
  },
];

export default routes;
