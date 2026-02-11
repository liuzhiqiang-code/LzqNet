using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.Test.Contracts.TestContent.Queries;

public record TestContentListQuery : Query<List<TestContentViewDto>>
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

    public override List<TestContentViewDto> Result { get; set; }

    public TestContentListQuery()
    {
    }
}