using LzqNet.Common.Contracts;
using SqlSugar;

namespace LzqNet.Test.Domain.Entities;

[Tenant("MsmConnection"), SugarTable("test_content_two")]
public class TestContentEntity : BaseFullEntity
{
    /// <summary>
    /// Name
    /// </summary>
    [SugarColumn(ColumnName = "name", Length = 100)]
    public string Name { get; set; }

    /// <summary>
    /// Remark
    /// </summary>
    [SugarColumn(ColumnName = "remark", Length = 2000)]
    public string? Remark { get; set; }

}