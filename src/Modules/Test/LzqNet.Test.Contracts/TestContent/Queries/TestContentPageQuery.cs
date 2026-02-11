using LzqNet.Caller.Common.Contracts;
using Masa.Utils.Models;

namespace LzqNet.Test.Contracts.TestContent.Queries;

public record TestContentPageQuery : PageQuery<TestContentViewDto>
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

    public override PaginatedListBase<TestContentViewDto> Result { get; set; }

    public TestContentPageQuery()
    {
    }
}