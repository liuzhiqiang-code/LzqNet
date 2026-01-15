using LzqNet.Caller.Common.Contracts;
using System.ComponentModel.DataAnnotations.Schema;

namespace LzqNet.Services.Msm.Domain.Entities.Modeling;

[Table("msm_modeling")]
public class ModelingEntity : BaseModelingEntity
{
    [Column("modeling_id")]
    public override long Id { get; set; }
    public long ModelingId => Id;

    /// <summary>
    /// ModelingName
    /// </summary>
    [Column("modeling_name")]
    public string ModelingName { get; set; }

    /// <summary>
    /// TableName
    /// </summary>
    [Column("table_name")]
    public string TableName { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [Column("description")]
    public string Description { get; set; }

}