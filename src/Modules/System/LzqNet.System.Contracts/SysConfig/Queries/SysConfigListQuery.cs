using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.System.Contracts.SysConfig.Queries;

public record SysConfigListQuery : Query<List<SysConfigViewDto>>
{
    public long? Id { get; set; }
    public override List<SysConfigViewDto> Result { get; set; }
    public SysConfigListQuery()
    {
    }
}
