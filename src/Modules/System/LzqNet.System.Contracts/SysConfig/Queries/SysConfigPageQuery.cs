using LzqNet.Common.Contracts;

namespace LzqNet.System.Contracts.SysConfig.Queries;

public record SysConfigPageQuery : PageQuery<SysConfigViewDto>
{
    public long? Id { get; set; }

    public override PageList<SysConfigViewDto> Result { get; set; }
    public SysConfigPageQuery()
    {
    }
}
