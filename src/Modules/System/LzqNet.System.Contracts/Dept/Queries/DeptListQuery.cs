using Masa.BuildingBlocks.ReadWriteSplitting.Cqrs.Queries;

namespace LzqNet.System.Contracts.Dept.Queries;

public record DeptListQuery : Query<List<DeptViewDto>>
{
    /// <summary>
    /// Id
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// Pid
    /// </summary>
    public long? Pid { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Status
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// Remark
    /// </summary>
    public string? Remark { get; set; }

    public override List<DeptViewDto> Result { get; set; }
    public DeptListQuery()
    {
    }
}