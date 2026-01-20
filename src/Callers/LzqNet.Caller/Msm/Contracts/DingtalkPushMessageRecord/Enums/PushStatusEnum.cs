namespace LzqNet.Caller.Msm.Contracts.DingtalkPushMessageRecord.Enums;

/// <summary>
/// 钉钉推送状态的枚举
/// </summary>
public enum DingtalkPushStatusEnum
{
    /// <summary>
    /// 待发布队列
    /// </summary>
    Pending = 1,

    /// <summary>
    /// 已发布队列
    /// </summary>
    Published = 2,

    /// <summary>
    /// 发送成功
    /// </summary>
    Success = 3,

    /// <summary>
    /// 发送失败
    /// </summary>
    Failed = 4
}
