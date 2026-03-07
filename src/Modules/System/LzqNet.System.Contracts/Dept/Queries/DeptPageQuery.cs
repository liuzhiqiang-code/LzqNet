using LzqNet.Common.Contracts;

namespace LzqNet.System.Contracts.Dept.Queries;

public record DeptPageQuery : PageQuery<DeptViewDto>
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

    public override PageList<DeptViewDto> Result { get; set; }
    public DeptPageQuery()
    {
    }
}