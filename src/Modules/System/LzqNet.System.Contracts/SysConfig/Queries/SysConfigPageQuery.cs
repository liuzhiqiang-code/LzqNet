using LzqNet.Caller.Common.Contracts;
using Masa.Utils.Models;

namespace LzqNet.System.Contracts.SysConfig.Queries;

public record SysConfigPageQuery : PageQuery<SysConfigViewDto>
{
    public long? Id { get; set; }

    public override PaginatedListBase<SysConfigViewDto> Result { get; set; }
    public SysConfigPageQuery()
    {
    }
}
