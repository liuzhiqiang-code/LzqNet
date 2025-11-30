import type { RouteRecordStringComponent } from '@vben/types';

import { msmRequestClient } from '#/api/request';

/**
 * 获取用户所有菜单
 */
export async function getAllMenusApi() {
  return msmRequestClient.get<RouteRecordStringComponent[]>('/menu/all');
}
