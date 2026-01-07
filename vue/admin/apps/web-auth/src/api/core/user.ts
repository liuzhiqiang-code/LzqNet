import type { UserInfo } from '@vben/types';

import { msmRequestClient } from '#/api/request';

/**
 * 获取用户信息
 */
export async function getUserInfoApi() {
  return msmRequestClient.get<UserInfo>('/account/userInfo');
}
