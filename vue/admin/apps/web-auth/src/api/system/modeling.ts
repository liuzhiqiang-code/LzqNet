import { msmRequestClient } from '#/api/request';
import type { SelectViewDto } from '#/api/types';

export namespace ModelingApi {
  export interface Modeling {
    [key: string]: any;
    keyword?: string;
    modelingName?: string;
    dataId?: string
  }
}

/**
 * 获取模型下拉列表
 */
async function getModelingSelectList(data: ModelingApi.Modeling) {
  return msmRequestClient.post<Array<SelectViewDto>>(
    '/modeling/selectlist',
    data
  );
}

const ModelingApiObj = {
  getSelectList: (data: ModelingApi.Modeling) => {
    return msmRequestClient.post<Array<SelectViewDto>>(
      `/modeling/${data.modelingName}/selectlist`,
      data
    );
  },
  getData: (data: ModelingApi.Modeling) => {
    return msmRequestClient.get<any>(
      `/modeling/${data.modelingName}/data?id=${data.dataId}`
    );
  }
}

export { getModelingSelectList, ModelingApiObj };
