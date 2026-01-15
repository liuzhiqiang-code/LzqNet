namespace LzqNet.Caller.Msm.Modeling.Model;

public class ModelViewDto
{
    public long Id { get; set; }
    public long ModelingId => Id;

    /// <summary>
    /// ModelingName
    /// </summary>
    public string ModelingName { get; set; }

    /// <summary>
    /// TableName
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// HasRevision
    /// </summary>
    public bool HasRevision { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; set; }

    public long Creator { get; set; }

    public DateTime CreationTime { get; set; }

    public long Modifier { get; set; }

    public DateTime ModificationTime { get; set; }

    public bool IsDeleted { get; set; }
}
