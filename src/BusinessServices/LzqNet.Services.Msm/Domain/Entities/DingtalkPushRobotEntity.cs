using LzqNet.Caller.Common.Contracts;
using SqlSugar;

namespace LzqNet.Services.Msm.Domain.Entities;

[SugarTable("msm_dingtalk_push_robot")]
public class DingtalkPushRobotEntity : BaseFullEntity
{
    /// <summary>
    /// 机器人名
    /// </summary>
    [SugarColumn(ColumnName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// 所属钉钉群组
    /// </summary>
    [SugarColumn(ColumnName = "dingtalk_group_name")]
    public string DingtalkGroupName { get; set; }

    /// <summary>
    /// 推送启用状态
    /// </summary>
    [SugarColumn(ColumnName = "enable_status")]
    public EnableStatusEnum EnableStatus { get; set; }

    /// <summary>
    /// 推送Webhook地址
    /// </summary>
    [SugarColumn(ColumnName = "webhook")]
    public string Webhook { get; set; }

    /// <summary>
    /// 推送关键词
    /// </summary>
    [SugarColumn(ColumnName = "push_keywords", IsJson = true)]
    public List<string> PushKeywords { get; set; }

    /// <summary>
    /// 加签
    /// </summary>
    [SugarColumn(ColumnName = "sign")]
    public string Sign { get; set; }

    /// <summary>
    /// 推送ip段
    /// </summary>
    [SugarColumn(ColumnName = "push_ip_segments", IsJson = true)]
    public List<string> PushIpSegments { get; set; }

}