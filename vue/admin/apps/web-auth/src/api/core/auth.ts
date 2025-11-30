import { baseAuthRequestClient, authRequestClient } from '#/api/request';

export namespace AuthApi {
  /** 登录接口参数 */
  export interface LoginParams {
    password?: string;
    username?: string;
    userName?: string;
  }

  /** 登录接口返回值 */
  export interface LoginResult {
    access_token: string;
  }

  export interface RefreshTokenResult {
    data: string;
    status: number;
  }
}

/**
 * 登录
 */
export async function loginApi(data: AuthApi.LoginParams) {
  data.userName = data.username;
  return await authRequestClient.post<AuthApi.LoginResult>('/Account/Login', data);
}

/**
 * 刷新accessToken
 */
export async function refreshTokenApi() {
  return baseAuthRequestClient.post<AuthApi.RefreshTokenResult>('/auth/refresh', {
    withCredentials: true,
  });
}

/**
 * 退出登录
 */
export async function logoutApi() {
  return authRequestClient.post('/Account/Logout', {
    withCredentials: true,
  });
}

/**
 * 获取用户权限码
 */
export async function getAccessCodesApi() {
  return [];
  //return msmRequestClient.get<string[]>('/auth/codes');
}
