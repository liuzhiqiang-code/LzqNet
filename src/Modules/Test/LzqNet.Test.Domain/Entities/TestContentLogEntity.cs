using LzqNet.Common.Contracts;
using SqlSugar;

namespace LzqNet.Test.Domain.Entities;

[Tenant("LogConnection"), SugarTable("test_content_log")]
public class TestContentLogEntity : BaseFullEntity
{
    /// <summary>
    /// Name
    /// </summary>
    [SugarColumn(ColumnName = "name")]
    public string Name { get; set; }

    /// <summary>
    /// Remark
    /// </summary>
    [SugarColumn(ColumnName = "remark")]
    public string? Remark { get; set; }

}