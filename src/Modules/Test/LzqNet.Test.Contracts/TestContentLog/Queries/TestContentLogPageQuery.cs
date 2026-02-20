using LzqNet.Common.Contracts;
using Masa.Utils.Models;

namespace LzqNet.Test.Contracts.TestContentLog.Queries;

public record TestContentLogPageQuery : PageQuery<TestContentLogViewDto>
{
    /// <summary>
    /// Id
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Remark
    /// </summary>
    public string? Remark { get; set; }

    public override PaginatedListBase<TestContentLogViewDto> Result { get; set; }

    public TestContentLogPageQuery()
    {
    }
}