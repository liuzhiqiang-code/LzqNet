import { msmRequestClient } from '#/api/request';

export namespace ModelingApi {
  export interface Modeling {
    [key: string]: any;
    id: string;
    name: string;
    remark?: string;
    status: 0 | 1;
  }
}

/**
 * 获取部门列表数据
 */
async function getDeptList() {
  return msmRequestClient.post<Array<ModelingApi.Modeling>>(
    '/dept/list',
  );
}

/**
 * 创建部门
 * @param data 部门数据
 */
async function createDept(
  data: Omit<ModelingApi.Modeling, 'children' | 'id'>,
) {
  return msmRequestClient.post('/dept/create', data);
}

/**
 * 更新部门
 *
 * @param id 部门 ID
 * @param data 部门数据
 */
async function updateDept(
  data: Omit<ModelingApi.Modeling, 'children'>,
) {
  return msmRequestClient.put(`/dept/update`, data);
}

/**
 * 删除部门
 * @param id 部门 ID
 */
async function deleteDept(id: string) {
  return msmRequestClient.delete(`/dept/delete/${id}`);
}

export { createDept, deleteDept, getDeptList, updateDept };
