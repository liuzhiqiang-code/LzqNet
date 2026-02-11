using LzqNet.Caller.Common.Contracts;
using Masa.Utils.Models;

namespace LzqNet.DingtalkMessage.Contracts.DingtalkPushConfig.Queries;

public record DingtalkPushConfigPageQuery : PageQuery<DingtalkPushConfigViewDto>
{
    /// <summary>
    /// Id
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// 关联业务Id
    /// </summary>
    public long? PushBusinessId { get; set; }

    /// <summary>
    /// 推送机器人Id
    /// </summary>
    public string? PushRobotIds { get; set; }

    /// <summary>
    /// 推送配置名
    /// </summary>
    public string? PushConfigName { get; set; }

    /// <summary>
    /// 推送类型
    /// </summary>
    public int? PushConfigType { get; set; }

    /// <summary>
    /// 推送模板
    /// </summary>
    public string? PushTemplate { get; set; }

    /// <summary>
    /// 推送启用状态
    /// </summary>
    public int? EnableStatus { get; set; }

    /// <summary>
    /// 关联钉钉用户
    /// </summary>
    public string? DingtalkUserIds { get; set; }

    public override PaginatedListBase<DingtalkPushConfigViewDto> Result { get; set; }
    public DingtalkPushConfigPageQuery()
    {
    }
}