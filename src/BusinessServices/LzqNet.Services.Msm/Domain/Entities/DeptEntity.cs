using LzqNet.Caller.Common.Contracts;
using SqlSugar;

namespace LzqNet.Services.Msm.Domain.Entities;

[SugarTable("msm_dept")]
public class DeptEntity : BaseFullEntity
{
    /// <summary>
    /// Pid
    /// </summary>
    [SugarColumn(ColumnName = "pid")]
    public long? Pid { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    [SugarColumn(ColumnName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    [SugarColumn(ColumnName = "status")]
    public EnableStatusEnum Status { get; set; }

    /// <summary>
    /// Remark
    /// </summary>
    [SugarColumn(ColumnName = "remark")]
    public string? Remark { get; set; }

}