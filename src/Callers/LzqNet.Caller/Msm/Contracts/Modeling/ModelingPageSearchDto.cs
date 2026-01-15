using Masa.Utils.Models;

namespace LzqNet.Caller.Msm.Contracts.Modeling;

public class ModelingPageSearchDto : RequestPageBase
{
    /// <summary>
    /// ModelingId
    /// </summary>
    public long? ModelingId { get; set; }

    /// <summary>
    /// ModelingName
    /// </summary>
    public string? ModelingName { get; set; }

    /// <summary>
    /// TableName
    /// </summary>
    public string? TableName { get; set; }

    /// <summary>
    /// HasRevision
    /// </summary>
    public bool? HasRevision { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string? Description { get; set; }

}