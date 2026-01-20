import { msmRequestClient } from '#/api/request';

export namespace DingtalkPushRobotApi {
  export interface DingtalkPushRobot {
    [key: string]: any;
    id:string;
    name:string;
    dingtalkGroupName:string;
    enableStatus:number;
    webhook:string;
    pushKeywords:string[];
    sign:string;
    pushIpSegments:string[];
    creator:string;
    creationTime:Date;
    modifier:string;
    modificationTime:Date;
  }
}

/**
 * 获取钉钉推送机器人列表数据
 */
async function getDingtalkPushRobotList() {
  return msmRequestClient.post<Array<DingtalkPushRobotApi.DingtalkPushRobot>>(
    '/dingtalk/PushRobot/list',
  );
}

/**
 * 创建钉钉推送机器人
 * @param data 钉钉推送机器人数据
 */
async function createDingtalkPushRobot(
  data: DingtalkPushRobotApi.DingtalkPushRobot,
) {
  return msmRequestClient.post('/dingtalk/PushRobot/create', data);
}

/**
 * 更新钉钉推送机器人
 *
 * @param id 钉钉推送机器人 ID
 * @param data 钉钉推送机器人数据
 */
async function updateDingtalkPushRobot(
  data: DingtalkPushRobotApi.DingtalkPushRobot,
) {
  return msmRequestClient.put(`/dingtalk/PushRobot/update`, data);
}

/**
 * 删除钉钉推送机器人
 * @param id 钉钉推送机器人 ID
 */
async function deleteDingtalkPushRobot(id: string) {
  return msmRequestClient.delete(`/dingtalk/PushRobot/delete/${id}`);
}

export { createDingtalkPushRobot, deleteDingtalkPushRobot, getDingtalkPushRobotList, updateDingtalkPushRobot };