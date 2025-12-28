import type { Recordable } from '@vben/types';

import { msmRequestClient } from '#/api/request';

export namespace SystemRoleApi {
  export interface SystemRole {
    [key: string]: any;
    id: string;
    name: string;
    permissions: string[];
    remark?: string;
    status: 0 | 1;
  }
}

/**
 * 获取角色列表数据
 */
async function getRoleList(params: Recordable<any>) {
  return msmRequestClient.post<Array<SystemRoleApi.SystemRole>>(
    '/role/page',
    { params },
  );
}

/**
 * 创建角色
 * @param data 角色数据
 */
async function createRole(data: Omit<SystemRoleApi.SystemRole, 'id'>) {
  return msmRequestClient.post('/role/create', data);
}

/**
 * 更新角色
 *
 * @param id 角色 ID
 * @param data 角色数据
 */
async function updateRole(data : Omit<SystemRoleApi.SystemRole, ''>) {
  return msmRequestClient.put(`/role/update`, data);
}

/**
 * 删除角色
 * @param id 角色 ID
 */
async function deleteRole(id: string) {
  return msmRequestClient.delete(`/role/delete/${id}`);
}

export { createRole, deleteRole, getRoleList, updateRole };
