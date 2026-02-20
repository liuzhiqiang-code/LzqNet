using LzqNet.Common.Contracts;
using SqlSugar;

namespace LzqNet.Test.Domain.Entities;

[Tenant("MsmConnection"), SugarTable("test_content")]
public class TestContentEntity : BaseFullEntity
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