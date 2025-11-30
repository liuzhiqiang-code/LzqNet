import type { UserInfo } from '@vben/types';

import { authRequestClient } from '#/api/request';

/**
 * 获取用户信息
 */
export async function getUserInfoApi() {
  return authRequestClient.get<UserInfo>('/Account/UserInfo');
}
