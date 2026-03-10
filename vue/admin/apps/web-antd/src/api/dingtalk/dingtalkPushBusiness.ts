import { requestClient } from '#/api/request';

export namespace DingtalkPushBusinessApi {
  export interface DingtalkPushBusiness {
    [key: string]: any;
    id:string;
    businessName:string;
    enableStatus:string;
    creator:string;
    creationTime:string;
    modifier:string;
    modificationTime:string;
  }
}

/**
 * 获取列表数据
 */
async function getDingtalkPushBusinessList() {
  return requestClient.post<Array<DingtalkPushBusinessApi.DingtalkPushBusiness>>(
    '/dingtalk/PushBusiness/list',
  );
}

/**
 * 创建
 * @param data 数据
 */
async function createDingtalkPushBusiness(
  data: DingtalkPushBusinessApi.DingtalkPushBusiness,
) {
  return requestClient.post('/dingtalk/PushBusiness/create', data);
}

/**
 * 更新
 *
 * @param id  ID
 * @param data 数据
 */
async function updateDingtalkPushBusiness(
  data: DingtalkPushBusinessApi.DingtalkPushBusiness,
) {
  return requestClient.put(`/dingtalk/PushBusiness/update`, data);
}

/**
 * 删除
 * @param id  ID
 */
async function deleteDingtalkPushBusiness(id: string) {
  return requestClient.delete(`/dingtalk/PushBusiness/delete/${id}`);
}

export { createDingtalkPushBusiness, deleteDingtalkPushBusiness, getDingtalkPushBusinessList, updateDingtalkPushBusiness };
