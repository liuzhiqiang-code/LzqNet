import { baseAuthRequestClient, msmRequestClient } from '#/api/request';

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
    refreshToken: string;
  }

  export interface RefreshTokenResult {
    data: string;
    status: number;
  }

  export interface RefreshTokenParams {
    refreshToken: null | string;
  }
}

/**
 * 登录
 */
export async function loginApi(data: AuthApi.LoginParams) {
  data.userName = data.username;
  return await msmRequestClient.post<AuthApi.LoginResult>('/account/login', data);
}

/**
 * 刷新accessToken
 */
export async function refreshTokenApi(data: AuthApi.RefreshTokenParams) {
  return baseAuthRequestClient.post<AuthApi.LoginResult>('/account/refresh', data);
}

/**
 * 退出登录
 */
export async function logoutApi() {
  return msmRequestClient.post('/account/logout', {
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
