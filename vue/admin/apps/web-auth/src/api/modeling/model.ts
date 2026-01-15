import { msmRequestClient } from '#/api/request';
import type { SelectViewDto } from '#/api/types';

export namespace ModelingApi {
  export interface Model {
    [key: string]: any;
    id: string;
    name: string;
    remark?: string;
    status: 0 | 1;
  }
}

/**
 * 获取Id/Name的下拉框数据列表 🔖
 */
async function getSelectList() {
  return msmRequestClient.post<Array<SelectViewDto>>(
    '/modeling/model/select',
  );
}

/**
 * 根据Id获取对应数据详情 🔖
 */
async function getData(id: string) {
  return msmRequestClient.get<ModelingApi.Model>(
    `/modeling/model/data?id=${id}`,
  );
}

/**
 * 创建部门
 * @param data 部门数据
 */
async function createData(
  data: ModelingApi.Model,
) {
  return msmRequestClient.post('/modeling/model/create', data);
}

/**
 * 更新部门
 *
 * @param id 部门 ID
 * @param data 部门数据
 */
async function updateData(
  data: ModelingApi.Model,
) {
  return msmRequestClient.put(`/modeling/model/update`, data);
}

/**
 * 删除部门
 * @param id 部门 ID
 */
async function deleteData(id: string) {
  return msmRequestClient.delete(`/modeling/model/delete/${id}`);
}

export default { getSelectList, getData, createData, updateData, deleteData };
