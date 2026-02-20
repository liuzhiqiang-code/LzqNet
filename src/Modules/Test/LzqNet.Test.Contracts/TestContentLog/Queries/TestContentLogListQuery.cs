using LzqNet.Test.Contracts.TestContentLog;
using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.Test.Contracts.TestContentLog.Queries;

public record TestContentLogListQuery : Query<List<TestContentLogViewDto>>
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

    public override List<TestContentLogViewDto> Result { get; set; }

    public TestContentLogListQuery()
    {
    }
}